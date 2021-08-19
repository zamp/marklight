using System;
using UnityEngine;

namespace MarkLight
{
    /// <summary>
    /// View field binding source.
    /// </summary>
    public class ViewFieldBindingSource : BindingSource
    {
        #region Fields

        public readonly string BindString;
        public readonly string Path;
        public readonly string RootFieldName;

        private static readonly string BoolTypeName = typeof(bool).Name;

        private readonly View _targetView; // The view the binding value targets for change.
        private View _targetLayoutParent;

        private View _sourceView; // The view the binding value is observed from.
        private View _sourceLayoutParent;
        private ViewFieldData _fieldData;
        private BindingProperties _properties;

        // 2-way binding
        private BindingValueObserver _targetObserver;
        private TwoWayBindingSource _targetSource;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ViewFieldBindingSource(View targetView, string bindingString)
        {
            _targetView = targetView;
            BindString = bindingString;
            Path = ParseBindingString(bindingString, out _properties);
            RootFieldName = Path.Split('.')[0];
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets value from binding source.
        /// </summary>
        public override object GetValue(out bool hasValue)
        {
            // Check if source view or target view has been moved to a new layout parent.
            if (!IsLocal &&
                (_sourceView.IsDestroyed || _targetView.IsDestroyed
                 || _sourceView.LayoutParent != _sourceLayoutParent
                 || _targetView.LayoutParent != _targetLayoutParent))
            {
                // re-initialize
                if (!OnObserverChange(Observer, Observer))
                {
                    return GetDefaultValue(out hasValue);
                }
            }

            if (_fieldData == null)
            {
                return GetDefaultValue(out hasValue);
            }

            var value = _fieldData.GetValue(out hasValue);

            if (!hasValue)
                value = GetDefaultValue(out hasValue);

            // check if value is to be negated
            if (IsNegated && _fieldData.TypeName == BoolTypeName)
            {
                value = !(bool)value;
            }

            if (value == null)
            {
                bool hasDefaultValue;
                var defaultValue = GetDefaultValue(out hasDefaultValue);
                if (hasDefaultValue)
                {
                    hasValue = true;
                    return defaultValue;
                }
            }

            return value;
        }

        /// <summary>
        /// Get the default value. Converts value if the source field has a converter.
        /// </summary>
        protected virtual object GetDefaultValue(out bool hasValue) {

            hasValue = false;

            if (DefaultValue == null)
                return null;

            if (_fieldData != null && _fieldData.ValueConverter != null)
            {
                var convertResult = _fieldData.ValueConverter.Convert(DefaultValue);
                hasValue = convertResult.Success;
                return convertResult.ConvertedValue;
            }

            hasValue = DefaultValue != null;
            return DefaultValue;
        }

        protected override bool OnObserverChange(BindingValueObserver oldObserver, BindingValueObserver observer)
        {
            // unregister old observer from fields
            if (oldObserver != null)
            {
                if (_fieldData != null)
                    _fieldData.UnregisterValueObserver(oldObserver);

                if (_targetObserver != null)
                    oldObserver.Target.UnregisterValueObserver(_targetObserver);
            }

            if (observer == null || _targetView.IsDestroyed)
                return false;

            // Find the view that the source field value should be taken from.
            _sourceView = FindBindingSourceView();
            if (_sourceView == null)
            {
                Debug.LogError(String.Format(
                    "[MarkLight] {0}: Unable to assign binding \"{1}\" to view field \"{2}\". " +
                    "Failed to find a target View that matches the binding.",
                    _targetView.GameObjectName, Path, RootFieldName));
                return false;
            }

            _targetLayoutParent = IsLocal ? null : _targetView.LayoutParent;
            _sourceLayoutParent = IsLocal ? null : _sourceView.LayoutParent;

            // get view field data for binding
            _fieldData = _sourceView.Fields.GetData(Path);
            if (_fieldData == null)
            {
                Debug.LogError(String.Format(
                    "[MarkLight] {0}: Unable to assign binding \"{1}\" to view field \"{2}\". " +
                    "Source binding view field \"{3}\" not found.",
                    _targetView.GameObjectName, BindString, Path, RootFieldName));
                return false;
            }

            // register observers to field data
            _fieldData.RegisterValueObserver(observer);

            // handle two-way bindings
            if (!observer.IsOneWayOnly && !IsOneWay)
            {
                // create value observer for target
                _targetObserver =
                    new SingleBindingValueObserver(_fieldData);

                _targetSource = new TwoWayBindingSource(observer.Target, IsNegated);
                _targetSource.SetObserver(_targetObserver);

                observer.Target.RegisterValueObserver(_targetObserver);

                // if this is a local binding and target view is the same as source view
                // we need to make sure value propagation happens in an intuitive order
                // so that if we e.g. bind Text="{#Item.Score}" that Item.Score propagates to Text first.
                if (IsLocalSearch && _fieldData.TargetView == _sourceView)
                {
                    _fieldData.IsPropagatedFirst = true;
                }
            }

            return true;
        }

        /// <summary>
        /// Find the View that a binding is referring to.
        /// </summary>
        private View FindBindingSourceView() {

            if (!IsLocalSearch && !IsParentSearch)
                return _targetView.Parent;

            var view = IsLocalSearch
                ? _targetView
                : _targetView.LayoutParent;

            while (view != null && view != ViewPresenter.Instance)
            {
                var fieldInfo = view.ViewTypeData.GetViewField(RootFieldName);
                if (fieldInfo != null)
                    return view;

                // don't search past parent if only doing local binding
                if (!IsParentSearch && view == _targetView.Parent)
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

            var components = binding.Substring(i).Split(':');
            if (components.Length > 2)
            {
                properties.DefaultValue = String.Join(":", components, 1, components.Length - 1);
            }
            else if (components.Length == 2)
            {
                properties.DefaultValue = components[1];
            }

            return components[0];
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets binding source string.
        /// </summary>
        public override string BindingSourceString
        {
            get
            {
                return String.Format("{0}.{1}", _fieldData.SourceView.ViewTypeName, _fieldData.Path);
            }
        }

        /// <summary>
        /// Get the view field data of the source field.
        /// </summary>
        public ViewFieldData FieldData
        {
            get { return _fieldData; }
        }

        /// <summary>
        /// Determine if the target of the binding is the same as the binding source view.
        /// </summary>
        public bool IsLocal
        {
            get { return _sourceView == _targetView; }
        }

        /// <summary>
        /// Determine if the binding has the local search modifier (#)
        /// </summary>
        public bool IsLocalSearch
        {
            get { return _properties.IsLocalSearch; }
        }

        /// <summary>
        /// Determine if the binding has the negate modifier (!)
        /// </summary>
        public bool IsNegated
        {
            get { return _properties.IsNegated; }
        }

        /// <summary>
        /// Determine if the binding has the one-way modifier (=)
        /// </summary>
        public bool IsOneWay
        {
            get { return _properties.IsOneWay; }
        }

        /// <summary>
        /// Determine if the binding has the resource modifier (@)
        /// </summary>
        public bool IsResource
        {
            get { return _properties.IsResource; }
        }

        /// <summary>
        /// Determine if the binding has the parent search modifier (^)
        /// </summary>
        public bool IsParentSearch
        {
            get { return _properties.IsParentSearch; }
        }

        /// <summary>
        /// Get the bindings default value specified using a colon (:) after the binding path followed by the
        /// default value.
        /// </summary>
        public string DefaultValue
        {
            get { return _properties.DefaultValue; }
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
            public string DefaultValue;
        }

        #endregion

        #region Classes

        private class TwoWayBindingSource : BindingSource
        {
            private readonly ViewFieldData _fieldData;
            private readonly bool _isNegated;

            public TwoWayBindingSource(ViewFieldData fieldData, bool isNegated) {
                _fieldData = fieldData;
                _isNegated = isNegated;
            }

            public override object GetValue(out bool hasValue) {

                if (_fieldData == null)
                {
                    hasValue = false;
                    return null;
                }

                var value = _fieldData.GetValue(out hasValue);

                // check if value is to be negated
                if (_isNegated && _fieldData.TypeName == BoolTypeName)
                {
                    value = !(bool)value;
                }

                return value;
            }

            public override string BindingSourceString
            {
                get { return String.Format("{0}.{1}", _fieldData.SourceView.ViewTypeName, _fieldData.Path); }
            }
        }

        #endregion
    }
}
