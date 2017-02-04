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
        public readonly View View;

        [SerializeField]
        private List<ViewFieldBinding> _bindings;

        private static readonly char[] DelimiterChars = {' ', ',', '$', '(', ')', '{', '}'};

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public ViewBindings(View view)
        {
            View = view;
            _bindings = new List<ViewFieldBinding>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called once to propagate bound values. Called in breadth-first order.
        /// </summary>
        public void Propagate()
        {
            foreach (var viewFieldData in View.Fields.Data.OrderByDescending(x => x.IsPropagatedFirst))
            {
                try
                {
                    viewFieldData.NotifyBindingValueObservers(new HashSet<ViewFieldData>());
                }
                catch (Exception e)
                {
                    Debug.LogError(String.Format(
                        "[MarkLight] {0}: Error while notifying binding value observers for field \"{1}\". {2}",
                        View.GameObjectName, viewFieldData.Path, Utils.GetError(e)));
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
            var fieldData = View.Fields.GetData(targetFieldName);
            if (fieldData == null)
            {
                Debug.LogError(String.Format(
                    "[MarkLight] {0}: Unable to assign binding \"{1}\" to view field \"{2}\". "+
                    "View field not found.",
                    View.GameObjectName, bindingString, targetFieldName));
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
            // create BindingValueObserver and add it as observer to source view fields
            var valueObserver = new BindingValueObserver
            {
                Target = fieldData
            };

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
                    View.GameObjectName, bindingString, targetFieldName));
                return;
            }

            // is the binding a format string?
            var isBindingFormatString = false;
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

                isBindingFormatString = true;
                valueObserver.BindingType = BindingType.MultiBindingFormatString;
                valueObserver.FormatString = formatString;
            }

            // parse view fields for binding source(s)
            foreach (var match in matches)
            {
                var binding = match.Groups["field"].Value.Trim();

                BindingProperties properties;
                var sourceFieldName = ParseBindingString(binding, out properties);

                // is this a binding to a resource in a resource dictionary?
                if (properties.IsResource)
                {
                    // yes.
                    SetResourceBinding(valueObserver, sourceFieldName);
                    continue;
                }

                // if the binding is defined as a local field (through the '#' notation) we are binding to a
                // field on this view otherwise we are binding to our parent view
                var bindingView = FindBindingView(sourceFieldName, properties);
                if (bindingView == null)
                {
                    Debug.LogError(String.Format(
                        "[MarkLight] {0}: Unable to assign binding \"{1}\" to view field \"{2}\". " +
                        "Failed to find a target View that matches the binding.",
                        View.GameObjectName, binding, targetFieldName));
                    return;
                }

                // get view field data for binding
                var sourceFieldData = bindingView.Fields.GetData(sourceFieldName);
                if (sourceFieldData == null)
                {
                    Debug.LogError(String.Format(
                        "[MarkLight] {0}: Unable to assign binding \"{1}\" to view field \"{2}\". "+
                        "Source binding view field \"{3}\" not found.",
                        View.GameObjectName, bindingString, targetFieldName, sourceFieldName));
                    return;
                }
                //Debug.Log(String.Format("Creating binding {0} <-> {1}", sourceViewFieldData.ViewFieldPath, viewFieldData.ViewFieldPath));

                valueObserver.Sources.Add(new ViewFieldBindingSource(sourceFieldData, properties.IsNegated));
                sourceFieldData.RegisterValueObserver(valueObserver);

                // handle two-way bindings
                if (!isBindingFormatString && !properties.IsOneWay)
                {
                    valueObserver.BindingType = BindingType.SingleBinding;

                    // create value observer for target
                    var targetObserver =
                        new BindingValueObserver
                        {
                            BindingType = BindingType.SingleBinding,
                            Target = sourceFieldData
                        };
                    targetObserver.Sources.Add(
                        new ViewFieldBindingSource(fieldData, properties.IsNegated));

                    fieldData.RegisterValueObserver(targetObserver);

                    // if this is a local binding and target view is the same as source view
                    // we need to make sure value propagation happens in an intuitive order
                    // so that if we e.g. bind Text="{#Item.Score}" that Item.Score propagates to Text first.
                    if (properties.IsLocalSearch && fieldData.TargetView == bindingView)
                    {
                        sourceFieldData.IsPropagatedFirst = true;
                    }
                }
            }
        }

        /// <summary>
        /// Parse and set method call multi binding.
        /// </summary>
        private void SetMethodMultiBinding(string targetFieldName, ViewFieldData fieldData, string bindingString)
        {
            // create BindingValueObserver and add it as observer to source view fields
            var valueObserver = new BindingValueObserver
            {
                Target = fieldData
            };

            // transformed multi-binding
            var bindings = bindingString.Split(DelimiterChars, StringSplitOptions.RemoveEmptyEntries);
            if (bindings.Length < 1)
            {
                Debug.LogError(String.Format(
                    "[MarkLight] {0}: Unable to assign binding \"{1}\" to view field \"{2}\". "+
                    "Improperly formatted binding string.",
                    View.GameObjectName, bindingString, targetFieldName));
                return;
            }

            valueObserver.BindingType = BindingType.MultiBindingTransform;
            valueObserver.ParentView = View.Parent;

            // get transformation method
            var transformMethodName = bindings[0];
            var transformMethodViewType = View.Parent.GetType();

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
                        View.GameObjectName, bindingString, targetFieldName, transformStr[0]));
                    return;
                }
            }

            try
            {
                valueObserver.TransformMethod = transformMethodViewType.GetMethod(transformMethodName,
                    BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public |
                    BindingFlags.NonPublic | BindingFlags.Static);
            }
            catch (Exception e)
            {
                Debug.LogError(String.Format(
                    "[MarkLight] {0}: Unable to assign binding \"{1}\" to view field \"{2}\". "+
                    "Error assigning transform method \"{3}\" in view type \"{4}\". {5}",
                    View.GameObjectName, bindingString, targetFieldName, bindings[0], View.Parent.ViewTypeName,
                    Utils.GetError(e)));
                return;
            }

            if (valueObserver.TransformMethod == null)
            {
                Debug.LogError(String.Format(
                    "[MarkLight] {0}: Unable to assign binding \"{1}\" to view field \"{2}\". "+
                    "Transform method \"{3}\" not found in view type \"{4}\".",
                    View.GameObjectName, bindingString, targetFieldName, bindings[0], View.Parent.ViewTypeName));
                return;
            }

            foreach (var binding in bindings.Skip(1))
            {
                BindingProperties properties;
                var sourceFieldName = ParseBindingString(binding, out properties);

                // is this a binding to a resource in a resource dictionary?
                if (properties.IsResource)
                {
                    // yes.
                    SetResourceBinding(valueObserver, sourceFieldName);
                    continue;
                }

                // if the binding is defined as a local field (through the '#' notation) we are binding to a
                // field on this view otherwise we are binding to our parent view
                var bindingView = FindBindingView(sourceFieldName, properties);
                if (bindingView == null)
                {
                    Debug.LogError(String.Format(
                        "[MarkLight] {0}: Unable to assign binding \"{1}\" to view field \"{2}\". " +
                        "Failed to find a target View that matches the binding.",
                        View.GameObjectName, binding, targetFieldName));
                    return;
                }

                // get view field data for binding
                var sourceFieldData = bindingView.Fields.GetData(sourceFieldName);
                if (sourceFieldData == null)
                {
                    Debug.LogError(String.Format(
                        "[MarkLight] {0}: Unable to assign binding \"{1}\" to view field \"{2}\". "+
                        "Source binding view field \"{3}\" not found.",
                        View.GameObjectName, bindingString, targetFieldName, binding));
                    return;
                }
                //Debug.Log(String.Format("Creating binding {0} <-> {1}", sourceViewFieldData.ViewFieldPath, viewFieldData.ViewFieldPath));

                valueObserver.Sources.Add(new ViewFieldBindingSource(sourceFieldData, properties.IsNegated));
                sourceFieldData.RegisterValueObserver(valueObserver);
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
            observer.Sources.Add(bindingSource);

            // so here we want to register a resource binding observer in the dictionary
            ResourceDictionary.RegisterResourceBindingObserver(dictionaryName, resourceKey, observer);
        }

        /// <summary>
        /// Find the View that a binding is referring to.
        /// </summary>
        private View FindBindingView(string bindFieldName, BindingProperties properties) {

            if (!properties.IsLocalSearch && !properties.IsParentSearch)
                return View.Parent;

            var rootFieldName = bindFieldName.Split('.')[0];

            var view = properties.IsLocalSearch
                ? View
                : View.LayoutParent;

            while (view != null && view != ViewPresenter.Instance)
            {
                var fieldInfo = view.ViewTypeData.GetViewField(rootFieldName);
                if (fieldInfo != null)
                    return view;

                // don't search past parent if only doing local binding
                if (!properties.IsParentSearch && view == View.Parent)
                    return null;

                // try next layout parent
                view = view.LayoutParent;
            }

            // search failed.
            return null;
        }

        /// <summary>
        /// Parses binding string and returns view field path.
        /// </summary>
        private static string ParseBindingString(string binding, out BindingProperties properties)
        {
            properties = new BindingProperties();

            var i = 0;
            for (; i < binding.Length; i++)
            {
                var ch = binding[i];
                var isDone = false;
                switch (ch)
                {
                    case '#':
                        properties.IsLocalSearch = true;
                        break;
                    case '!':
                        properties.IsNegated = true;
                        break;
                    case '=':
                        properties.IsOneWay = true;
                        break;
                    case '@':
                        properties.IsResource = true;
                        break;
                    case '^':
                        properties.IsParentSearch = true;
                        break;
                    default:
                        isDone = true;
                        break;
                }
                if (isDone)
                    break;
            }

            return binding.Substring(i);
        }

        #endregion

        #region Structs

        private struct BindingProperties
        {
            public bool IsLocalSearch;
            public bool IsNegated;
            public bool IsOneWay;
            public bool IsResource;
            public bool IsParentSearch;
        }

        #endregion
    }
}