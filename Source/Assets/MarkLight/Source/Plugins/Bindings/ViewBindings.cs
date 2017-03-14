using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace MarkLight
{
    /// <summary>
    /// Bindings manager for a View.
    /// </summary>
    [Serializable]
    public class ViewBindings
    {
        #region Fields

        [NonSerialized]
        public readonly View OwnerView;

        [NonSerialized]
        private IObservableItem _item;

        [SerializeField]
        private List<ViewFieldBinding> _bindings;

        private static readonly char[] DelimiterChars = {' ', ',', '$', '(', ')', '{', '}'};

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public ViewBindings(View ownerView)
        {
            OwnerView = ownerView;
            _bindings = new List<ViewFieldBinding>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called once to propagate bound values. Called in breadth-first order.
        /// </summary>
        public void Propagate()
        {
            foreach (var viewFieldData in OwnerView.Fields.Data.OrderByDescending(x => x.IsPropagatedFirst))
            {
                try
                {
                    viewFieldData.NotifyBindingValueObservers(new HashSet<ViewFieldData>());
                }
                catch (Exception e)
                {
                    Debug.LogError(String.Format(
                        "[MarkLight] {0}: Error while notifying binding value observers for field \"{1}\". {2}",
                        OwnerView.GameObjectName, viewFieldData.Path, Utils.GetError(e)));
                }
            }
        }

        /// <summary>
        /// Adds a binding to the view field that will be processed when the view is initialized.
        /// </summary>
        public void Add(string fieldPath, string bindingString)
        {
            _bindings.Add(new ViewFieldBinding(fieldPath, bindingString));
        }

        /// <summary>
        /// Sets view field binding.
        /// </summary>
        public void Set(string targetFieldName, string bindingString)
        {
            // get view field data for binding target
            var fieldData = OwnerView.Fields.GetData(targetFieldName);
            if (fieldData == null)
            {
                Debug.LogError(String.Format(
                    "[MarkLight] {0}: Unable to assign binding \"{1}\" to view field \"{2}\". "+
                    "View field not found.",
                    OwnerView.GameObjectName, bindingString, targetFieldName));
                return;
            }

            fieldData.SetIsSet();

            // parse view field binding string
            var trimmedBinding = bindingString.Trim();

            if (trimmedBinding.StartsWith("$"))
            {
                SetMethodMultiBinding(targetFieldName, fieldData, trimmedBinding);
            }
            else
            {
                SetSingleBinding(targetFieldName, fieldData, trimmedBinding);
            }
        }

        /// <summary>
        /// Called by the view when its internals are initialzed.
        /// </summary>
        internal void InitializeInternal()
        {
            foreach (var binding in _bindings)
            {
                Set(binding.FieldPath, binding.BindingString);
            }
        }

        /// <summary>
        /// Parse and set single binding.
        /// </summary>
        private void SetSingleBinding(string targetFieldName, ViewFieldData fieldData, string bindingString)
        {
            // check for bindings in string
            var matches = new List<Match>();
            foreach (Match match in ViewFieldBinding.BindingRegex.Matches(bindingString))
            {
                matches.Add(match);
            }

            if (matches.Count <= 0)
            {
                // no bindings found
                Debug.LogError(String.Format(
                    "[MarkLight] {0}: Unable to assign binding \"{1}\" to view field \"{2}\". "+
                    "String contains no binding.",
                    OwnerView.GameObjectName, bindingString, targetFieldName));
                return;
            }

            // create BindingValueObserver and add it as observer to source view fields
            BindingValueObserver valueObserver;

            // is the binding a format string?
            if (matches.Count > 1 ||
                matches[0].Value.Length != bindingString.Length ||
                !String.IsNullOrEmpty(matches[0].Groups["format"].Value))
            {
                // yes.
                var matchCount = 0;
                var formatString = ViewFieldBinding.BindingRegex.Replace(bindingString, x =>
                {
                    var matchCountString = matchCount.ToString();
                    ++matchCount;
                    return String.Format("{{{0}{1}}}", matchCountString, x.Groups["format"]);
                });

                valueObserver = new MultiBindingFormatValueObserver(fieldData, formatString);
            }
            else
            {
                valueObserver = new SingleBindingValueObserver(fieldData);
            }

            // parse view fields for binding source(s)
            foreach (var match in matches)
            {
                var binding = match.Groups["field"].Value.Trim();

                var bindingSource = new ViewFieldBindingSource(OwnerView, binding);

                // is this a binding to a resource in a resource dictionary?
                if (bindingSource.IsResource)
                {
                    // yes.
                    SetResourceBinding(valueObserver, bindingSource.RootFieldName);
                    continue;
                }

                if (!bindingSource.SetObserver(valueObserver))
                    return;
            }
        }

        /// <summary>
        /// Parse and set method call multi binding.
        /// </summary>
        private void SetMethodMultiBinding(string targetFieldName, ViewFieldData fieldData, string bindingString)
        {
            // transformed multi-binding
            var bindings = bindingString.Split(DelimiterChars, StringSplitOptions.RemoveEmptyEntries);
            if (bindings.Length < 1)
            {
                Debug.LogError(String.Format(
                    "[MarkLight] {0}: Unable to assign binding \"{1}\" to view field \"{2}\". "+
                    "Improperly formatted binding string.",
                    OwnerView.GameObjectName, bindingString, targetFieldName));
                return;
            }

            // get transformation method
            var transformMethodName = bindings[0];
            var transformMethodViewType = OwnerView.Parent.GetType();

            var transformStr = bindings[0].Split('.');
            if (transformStr.Length == 2)
            {
                transformMethodViewType = ViewData.GetViewType(transformStr[0]);
                transformMethodName = transformStr[1];

                if (transformMethodViewType == null)
                {
                    Debug.LogError(String.Format(
                        "[MarkLight] {0}: Unable to assign binding \"{1}\" to view field \"{2}\". "+
                        "View \"{3}\" not found.",
                        OwnerView.GameObjectName, bindingString, targetFieldName, transformStr[0]));
                    return;
                }
            }

            MethodInfo transformMethod;

            try
            {
                transformMethod = transformMethodViewType.GetMethod(transformMethodName,
                    BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public |
                    BindingFlags.NonPublic | BindingFlags.Static);
            }
            catch (Exception e)
            {
                Debug.LogError(String.Format(
                    "[MarkLight] {0}: Unable to assign binding \"{1}\" to view field \"{2}\". "+
                    "Error assigning transform method \"{3}\" in view type \"{4}\". {5}",
                    OwnerView.GameObjectName, bindingString, targetFieldName, bindings[0], OwnerView.Parent.ViewTypeName,
                    Utils.GetError(e)));
                return;
            }

            if (transformMethod == null)
            {
                Debug.LogError(String.Format(
                    "[MarkLight] {0}: Unable to assign binding \"{1}\" to view field \"{2}\". " +
                    "Transform method \"{3}\" not found in view type \"{4}\".",
                    OwnerView.GameObjectName, bindingString, targetFieldName, bindings[0],
                    OwnerView.Parent.ViewTypeName));
                return;
            }

            // create BindingValueObserver and add it as observer to source view fields
            BindingValueObserver valueObserver = new MultiBindingTransformValueObserver(
                fieldData, transformMethod, OwnerView.Parent);

            foreach (var binding in bindings.Skip(1))
            {
                var bindingSource = new ViewFieldBindingSource(OwnerView, binding);

                // is this a binding to a resource in a resource dictionary?
                if (bindingSource.IsResource)
                {
                    // yes.
                    SetResourceBinding(valueObserver, bindingSource.RootFieldName);
                    continue;
                }

                if (!bindingSource.SetObserver(valueObserver))
                    return;
            }
        }

        /// <summary>
        /// Sets resource binding.
        /// </summary>
        private static void SetResourceBinding(BindingValueObserver observer, string sourceFieldName)
        {
            string dictionaryName = null;
            var resourceKey = sourceFieldName;

            var resourceIndex = sourceFieldName.IndexOf('.', 0);
            if (resourceIndex > 0)
            {
                resourceKey = sourceFieldName.Substring(resourceIndex + 1);
                dictionaryName = sourceFieldName.Substring(0, resourceIndex);
            }

            var bindingSource = new ResourceBindingSource(dictionaryName, resourceKey);
            bindingSource.SetObserver(observer);

            // so here we want to register a resource binding observer in the dictionary
            ResourceDictionary.RegisterResourceBindingObserver(dictionaryName, resourceKey, observer);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get data bound item.
        /// </summary>
        public IObservableItem Item
        {
            get
            {
                if (_item != null)
                    return _item;

                var parentView = OwnerView.LayoutParent;
                return parentView != null ? parentView.Bindings.Item : _item;
            }
            set
            {
                var old = _item;
                _item = value;
                OwnerView.DataModelItemChanged(old, _item);
                OwnerView.ForEachChild<View>(x =>
                {
                    if (x.Bindings._item != null)
                    {
                        // skip child data model items and their descendants
                        return false;
                    }

                    x.DataModelItemChanged(old, _item);
                    return true;
                }, ViewSearchArgs.ContinueOnFalsePredicate);
            }
        }

        #endregion
    }
}