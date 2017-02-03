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
        private List<ViewFieldBinding> _viewFieldBindings;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public ViewBindings(View view) {
            View = view;
            _viewFieldBindings = new List<ViewFieldBinding>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Called once to propagate bound values. Called in breadth-first order.
        /// </summary>
        public void Propagate()
        {
            foreach (var viewFieldData in View.FieldDataValues.OrderByDescending(x => x.IsPropagatedFirst))
            {
                viewFieldData.NotifyBindingValueObservers(new HashSet<ViewFieldData>());
            }
        }

        /// <summary>
        /// Adds a binding to the view field that will be processed when the view is initialized.
        /// </summary>
        public void Add(string viewField, string bindingString)
        {
            _viewFieldBindings.Add(new ViewFieldBinding(bindingString, viewField));
        }

        /// <summary>
        /// Sets view field binding.
        /// </summary>
        public void Set(string viewField, string viewFieldBinding) {
            // get view field data for binding target
            var viewFieldData = View.GetViewFieldData(viewField);
            if (viewFieldData == null)
            {
                Debug.LogError(String.Format(
                    "[MarkLight] {0}: Unable to assign binding \"{1}\" to view field \"{2}\". View field not found.",
                    View.GameObjectName, viewFieldBinding, viewField));
                return;
            }

            viewFieldData.SetIsSet();

            // create BindingValueObserver and add it as observer to source view fields
            var valueObserver = new BindingValueObserver
            {
                Target = viewFieldData
            };

            // parse view field binding string
            char[] delimiterChars = {' ', ',', '$', '(', ')', '{', '}'};
            var trimmedBinding = viewFieldBinding.Trim();

            if (trimmedBinding.StartsWith("$"))
            {
                // transformed multi-binding
                string[] bindings = trimmedBinding.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);
                if (bindings.Length < 1)
                {
                    Debug.LogError(String.Format(
                        "[MarkLight] {0}: Unable to assign binding \"{1}\" to view field \"{2}\". Improperly "+
                        "formatted binding string.",
                        View.GameObjectName, viewFieldBinding, viewField));
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
                            View.GameObjectName, viewFieldBinding, viewField, transformStr[0]));
                        return;
                    }
                }

                try
                {
                    valueObserver.TransformMethod = transformMethodViewType.GetMethod(transformMethodName,
                        BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
                        BindingFlags.Static);
                }
                catch (Exception e)
                {
                    Debug.LogError(String.Format(
                        "[MarkLight] {0}: Unable to assign binding \"{1}\" to view field \"{2}\". Error assigning "+
                        "transform method \"{3}\" in view type \"{4}\". {5}",
                        View.GameObjectName, viewFieldBinding, viewField, bindings[0], View.Parent.ViewTypeName,
                        Utils.GetError(e)));
                    return;
                }

                if (valueObserver.TransformMethod == null)
                {
                    Debug.LogError(String.Format(
                        "[MarkLight] {0}: Unable to assign binding \"{1}\" to view field \"{2}\". Transform "+
                        "method \"{3}\" not found in view type \"{4}\".",
                        View.GameObjectName, viewFieldBinding, viewField, bindings[0], View.Parent.ViewTypeName));
                    return;
                }

                foreach (var binding in bindings.Skip(1))
                {
                    bool isLocalField;
                    bool isNegatedField;
                    bool isOneWay;
                    bool isResource;

                    var sourceFieldName = ParseBindingString(binding, out isLocalField, out isNegatedField,
                        out isOneWay, out isResource);

                    // is this a binding to a resource in a resource dictionary?
                    if (isResource)
                    {
                        // yes.
                        SetResourceBinding(valueObserver, sourceFieldName);
                        continue;
                    }

                    // if the binding is defined as a local field (through the '#' notation) we are binding to a
                    // field on this view otherwise we are binding to our parent view
                    var bindingView = isLocalField ? View : View.Parent;

                    // get view field data for binding
                    var sourceViewFieldData = bindingView.GetViewFieldData(sourceFieldName);
                    if (sourceViewFieldData == null)
                    {
                        Debug.LogError(String.Format(
                            "[MarkLight] {0}: Unable to assign binding \"{1}\" to view field \"{2}\". "+
                            "Source binding view field \"{3}\" not found.",
                            View.GameObjectName, viewFieldBinding, viewField, binding));
                        return;
                    }
                    //Debug.Log(String.Format("Creating binding {0} <-> {1}", sourceViewFieldData.ViewFieldPath, viewFieldData.ViewFieldPath));

                    valueObserver.Sources.Add(new ViewFieldBindingSource(sourceViewFieldData, isNegatedField));
                    sourceViewFieldData.RegisterValueObserver(valueObserver);
                }
            }
            else
            {
                // check for bindings in string
                var matches = new List<Match>();
                foreach (Match match in ViewFieldBinding.BindingRegex.Matches(viewFieldBinding))
                {
                    matches.Add(match);
                }

                if (matches.Count <= 0)
                {
                    // no bindings found
                    Debug.LogError(String.Format(
                        "[MarkLight] {0}: Unable to assign binding \"{1}\" to view field \"{2}\". "+
                        "String contains no binding.",
                        View.GameObjectName, viewFieldBinding, viewField));
                    return;
                }

                // is the binding a format string?
                var isBindingFormatString = false;
                if (matches.Count > 1 || matches[0].Value.Length != viewFieldBinding.Length ||
                    !String.IsNullOrEmpty(matches[0].Groups["format"].Value))
                {
                    // yes.
                    var matchCount = 0;
                    var formatString = ViewFieldBinding.BindingRegex.Replace(viewFieldBinding, x =>
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
                    bool isLocalField;
                    bool isNegatedField;
                    bool isOneWay;
                    bool isResource;

                    var sourceFieldName = ParseBindingString(binding, out isLocalField, out isNegatedField,
                        out isOneWay, out isResource);

                    // is this a binding to a resource in a resource dictionary?
                    if (isResource)
                    {
                        // yes.
                        SetResourceBinding(valueObserver, sourceFieldName);
                        continue;
                    }

                    // if the binding is defined as a local field (through the '#' notation) we are binding to a
                    // field on this view otherwise we are binding to our parent view
                    var bindingView = isLocalField ? View : View.Parent;

                    // get view field data for binding
                    var sourceFieldData = bindingView.GetViewFieldData(sourceFieldName);
                    if (sourceFieldData == null)
                    {
                        Debug.LogError(String.Format(
                            "[MarkLight] {0}: Unable to assign binding \"{1}\" to view field \"{2}\". "+
                            "Source binding view field \"{3}\" not found.",
                            View.GameObjectName, viewFieldBinding, viewField, sourceFieldName));
                        return;
                    }
                    //Debug.Log(String.Format("Creating binding {0} <-> {1}", sourceViewFieldData.ViewFieldPath, viewFieldData.ViewFieldPath));

                    valueObserver.Sources.Add(new ViewFieldBindingSource(sourceFieldData, isNegatedField));
                    sourceFieldData.RegisterValueObserver(valueObserver);

                    // handle two-way bindings
                    if (!isBindingFormatString && !isOneWay)
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
                            new ViewFieldBindingSource(viewFieldData, isNegatedField));

                        viewFieldData.RegisterValueObserver(targetObserver);
                        View.AddValueObserver(targetObserver);

                        // if this is a local binding and target view is the same as source view
                        // we need to make sure value propagation happens in an intuitive order
                        // so that if we e.g. bind Text="{#Item.Score}" that Item.Score propagates to Text first.
                        if (isLocalField && viewFieldData.TargetView == bindingView)
                        {
                            sourceFieldData.IsPropagatedFirst = true;
                        }
                    }
                }
            }

            View.AddValueObserver(valueObserver);
        }

        /// <summary>
        /// Called by the view when its internals are initialzed.
        /// </summary>
        internal void InitializeInternal()
        {
            foreach (var binding in _viewFieldBindings)
            {
                Set(binding.FieldName, binding.BindingString);
            }
        }

        /// <summary>
        /// Sets resource binding.
        /// </summary>
        private static void SetResourceBinding(BindingValueObserver bindingValueObserver, string sourceFieldName)
        {
            string dictionaryName = null;
            var resourceKey = sourceFieldName;

            var resourceIndex = sourceFieldName.IndexOf('.', 0);
            if (resourceIndex > 0)
            {
                resourceKey = sourceFieldName.Substring(resourceIndex + 1);
                dictionaryName = sourceFieldName.Substring(0, resourceIndex);
            }

            var resourceBindingSource = new ResourceBindingSource(dictionaryName, resourceKey);
            bindingValueObserver.Sources.Add(resourceBindingSource);

            // so here we want to register a resource binding observer in the dictionary
            ResourceDictionary.RegisterResourceBindingObserver(dictionaryName, resourceKey, bindingValueObserver);
        }

        /// <summary>
        /// Parses binding string and returns view field path.
        /// </summary>
        private static string ParseBindingString(string binding, out bool isLocalField, out bool isNegatedField,
                                                 out bool isOneWay, out bool isResource)
        {
            isLocalField = false;
            isNegatedField = false;
            isOneWay = false;
            isResource = false;

            var viewField = binding;
            while (viewField.Length > 0)
            {
                if (viewField.StartsWith("#"))
                {
                    isLocalField = true;
                    viewField = viewField.Substring(1);
                }
                else if (viewField.StartsWith("!"))
                {
                    isNegatedField = true;
                    viewField = viewField.Substring(1);
                }
                else if (viewField.StartsWith("="))
                {
                    isOneWay = true;
                    viewField = viewField.Substring(1);
                }
                else if (viewField.StartsWith("@"))
                {
                    isResource = true;
                    viewField = viewField.Substring(1);
                }
                else
                {
                    break;
                }
            }

            return viewField;
        }

        #endregion
    }
}