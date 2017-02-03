using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MarkLight
{
    /// <summary>
    /// Contains data about a view field.
    /// </summary>
    public class ViewFieldData
    {
        #region Fields

        public bool IsPropagatedFirst;

        private readonly View _sourceView;
        private readonly View _targetView;

        private ViewFieldPathInfo _pathInfo;
        private HashSet<ValueObserver> _valueObservers;
        private bool _isSet;
        private bool _isSetInitialized;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        private ViewFieldData(View sourceView, ViewFieldPathInfo pathInfo) {
            _sourceView = sourceView;
            _targetView = pathInfo.GetTargetView(sourceView);
            _pathInfo = pathInfo;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets field data from field path.
        /// </summary>
        public static ViewFieldData FromViewFieldPath(View sourceView, string path)
        {
            if (String.IsNullOrEmpty(path) || sourceView == null)
                return null;

            var viewTypeData = sourceView.ViewTypeData;

            // try cache first
            var pathInfo = viewTypeData.GetViewFieldPathInfo(path);
            if (pathInfo != null)
                return new ViewFieldData(sourceView, pathInfo);

            // create new path info
            pathInfo = new ViewFieldPathInfo(sourceView, path);

            // add path info if parsed successfully.
            if (pathInfo.IsParsed && !pathInfo.IsMappedToDescendants)
                viewTypeData.AddViewFieldPathInfo(pathInfo.Path, pathInfo);

            var result = new ViewFieldData(sourceView, pathInfo);

            // if no target view found, assume path refers to this view (in cases like x.SetValue(() => x.Field, value))
            return result.TargetView == null
                ? FromViewFieldPath(sourceView, String.Join(".", pathInfo.Fields.Skip(1).ToArray()))
                : result;
        }

        /// <summary>
        /// Determine if the field contains a specified dependency field path.
        /// </summary>
        public bool HasDependency(string path) {
            return _pathInfo.Dependencies.Contains(path);
        }

        /// <summary>
        /// Sets isSet-indicator.
        /// </summary>
        public void SetIsSet()
        {
            if (!_isSetInitialized)
            {
                SourceView.AddIsSetField(Path);
                _isSetInitialized = true;
            }
            _isSet = true;
        }

        /// <summary>
        /// Sets value of field.
        /// </summary>
        public object SetValue(object inValue, HashSet<ViewFieldData> callstack, bool updateDefaultState = true,
            ValueConverterContext context = null, bool notifyObservers = true, bool suppressAssignErrors = false)
        {
            if (callstack.Contains(this))
                return null;

            callstack.Add(this);

            if (IsMapped)
            {
                var targetView = GetTargetView();
                if (targetView != null)
                {
                    return targetView.SetValue(TargetPath, inValue, updateDefaultState, callstack, null,
                        notifyObservers);
                }

                if (!suppressAssignErrors)
                {
                    Debug.LogError(String.Format(
                        "[MarkLight] {0}: Unable to assign value \"{1}\" to view field \"{2}\". View along path is null.",
                        SourceView.GameObjectName, inValue, Path));
                }

                return null;
            }

            // check if path has been parsed
            if (!ParsePath())
            {
                // path can't be resolved at this point
                if (_pathInfo.IsSevereParseError && !suppressAssignErrors)
                {
                    // severe parse error means the path is incorrect
                    Debug.LogError(String.Format("[MarkLight] {0}: Unable to assign value \"{1}\". {2}",
                        SourceView.GameObjectName, inValue, Utils.ErrorMessage));
                }

                // unsevere parse errors can be expected, e.g. value along path is null
                return null;
            }

            var value = inValue;
            if (context == null)
            {
                context = SourceView.ValueConverterContext ?? ValueConverterContext.Default;
            }

            // get converted value            
            if (ValueConverter != null)
            {
                var conversionResult = ValueConverter.Convert(value, context);
                if (!conversionResult.Success)
                {
                    Debug.LogError(String.Format(
                        "[MarkLight] {0}: Unable to assign value \"{1}\" to view field \"{2}\". Value conversion failed. {3}",
                        SourceView.GameObjectName, value, Path, conversionResult.ErrorMessage));
                    return null;
                }
                value = conversionResult.ConvertedValue;
            }

            // set value
            var oldValue = _pathInfo.SetValue(SourceView, value);

            // notify observers if the value has changed
            if (notifyObservers)
            {
                // set isSet-indicator
                SetIsSet();

                var valueChanged = value != null ? !value.Equals(oldValue) : oldValue != null;
                if (valueChanged)
                {
                    NotifyValueObservers(callstack);

                    // find dependent view fields and notify their value observers
                    SourceView.NotifyDependentValueObservers(Path);
                }
            }

            return value;
        }

        /// <summary>
        /// Gets value of field.
        /// </summary>
        public object GetValue(out bool hasValue) {
            if (IsMapped)
            {
                var targetView = GetTargetView();
                if (targetView != null)
                    return targetView.GetValue(TargetPath, out hasValue);

                hasValue = false;
                //Debug.LogError(String.Format("[MarkLight] {0}: Unable to get value from view field \"{1}\". View along path is null.", SourceView.GameObjectName, ViewFieldPath)));
                return null;
            }
            // check if path has been parsed
            if (!ParsePath())
            {
                hasValue = false;
                return null;
            }

            return _pathInfo.GetValue(SourceView, out hasValue);
        }

        /// <summary>
        /// Registers a value observer.
        /// </summary>
        public void RegisterValueObserver(ValueObserver valueObserver)
        {
            if (_valueObservers == null)
            {
                _valueObservers = new HashSet<ValueObserver>();
            }

            _valueObservers.Add(valueObserver);
        }

        /// <summary>
        /// Notifies all value observers that value has been set.
        /// </summary>
        public void NotifyValueObservers(HashSet<ViewFieldData> callstack)
        {
            if (_valueObservers == null)
                return;

            List<ValueObserver> removedObservers = null;
            foreach (var valueObserver in _valueObservers)
            {
                // notify observer
                bool isRemoved = !valueObserver.Notify(callstack);
                if (isRemoved)
                {
                    if (removedObservers == null)
                    {
                        removedObservers = new List<ValueObserver>();
                    }

                    removedObservers.Add(valueObserver);
                }
            }

            if (removedObservers != null)
            {
                removedObservers.ForEach(x => _valueObservers.Remove(x));
            }
        }

        /// <summary>
        /// Notifies all binding value observers that value has been set.
        /// </summary>
        public void NotifyBindingValueObservers(HashSet<ViewFieldData> callstack)
        {
            if (_valueObservers == null)
                return;

            List<ValueObserver> removedObservers = null;
            foreach (var valueObserver in _valueObservers)
            {
                if (valueObserver is BindingValueObserver)
                {
                    bool isRemoved = !valueObserver.Notify(callstack);
                    if (isRemoved)
                    {
                        if (removedObservers == null)
                        {
                            removedObservers = new List<ValueObserver>();
                        }

                        removedObservers.Add(valueObserver);
                    }
                }
            }

            if (removedObservers != null)
            {
                removedObservers.ForEach(x => _valueObservers.Remove(x));
            }
        }

        /// <summary>
        /// Notifies all change handler value observers that value has been set.
        /// </summary>
        public void NotifyChangeHandlerValueObservers(HashSet<ViewFieldData> callstack)
        {
            if (_valueObservers == null)
                return;

            foreach (var valueObserver in _valueObservers)
            {
                if (valueObserver is ChangeHandlerValueObserver)
                {
                    valueObserver.Notify(callstack);
                }
            }
        }

        /// <summary>
        /// Gets target view. Only called if this view is mapped.
        /// </summary>
        public View GetTargetView()
        {
            if (IsMappedToDescendants)
            {
                return TargetView;
            }

            bool hasValue;
            return _pathInfo.GetValue(SourceView, out hasValue) as View;
        }

        /// <summary>
        /// Gets bool indicating if the view field has been set.
        /// </summary>
        public bool IsSet()
        {
            if (_isSetInitialized || _isSet)
                return _isSet;

            // check with source view if the view field has been set
            _isSetInitialized = true;
            _isSet = SourceView.GetIsSetFieldValue(Path);
            return _isSet;
        }

        private bool ParsePath() {

            if (IsParsed)
                return true;

            var viewTypeData = SourceView.ViewTypeData;
            var pathInfo = viewTypeData.GetViewFieldPathInfo(Path);
            if (pathInfo != null)
            {
                _pathInfo = pathInfo;
                return true;
            }

            if (_pathInfo.ParsePath(SourceView))
            {

                if (!_pathInfo.IsGeneric)
                    viewTypeData.AddViewFieldPathInfo(Path, _pathInfo);

                return true;
            }
            return false;
        }

        #endregion
        
        #region Properties

        /// <summary>
        /// Get the source view.
        /// </summary>
        public View SourceView
        {
            get { return _sourceView; }
        }

        /// <summary>
        /// Get the view that the field is targeting.
        /// </summary>
        public View TargetView
        {
            get { return _targetView; }
        }

        /// <summary>
        /// Gets view field value converter.
        /// </summary>
        public ValueConverter ValueConverter
        {
            get
            {
                return _pathInfo.ValueConverter;
            }
        }

        /// <summary>
        /// Gets view field type name.
        /// </summary>
        public string TypeName
        {
            get
            {
                return _pathInfo.TypeName;
            }
        }

        /// <summary>
        /// Gets view field type.
        /// </summary>
        public Type Type
        {
            get
            {
                return _pathInfo.Type;
            }
        }

        /// <summary>
        /// Gets view field path.
        /// </summary>
        public string Path
        {
            get
            {
                return _pathInfo.Path;
            }
        }

        /// <summary>
        /// Gets target view field path.
        /// </summary>
        public string TargetPath
        {
            get
            {
                return _pathInfo.TargetPath;
            }
        }

        /// <summary>
        /// Gets boolean indicating if path has been parsed.
        /// </summary>
        public bool IsParsed
        {
            get
            {
                return _pathInfo.IsParsed;
            }
        }

        /// <summary>
        /// Returns boolean indicating if this view field is the owner of the value (not mapped to another view).
        /// </summary>
        public bool IsMapped
        {
            get
            {
                return _pathInfo.IsMapped;
            }
        }

        /// <summary>
        /// Determine if the field is mapped to or through a descendant view.
        /// </summary>
        public bool IsMappedToDescendants
        {
            get { return _pathInfo.IsMappedToDescendants; }
        }

        #endregion
    }
}
