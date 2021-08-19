using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

namespace MarkLight
{
    /// <summary>
    /// View field manager.
    /// </summary>
    [Serializable]
    public class ViewFields
    {
        #region Fields

        [NonSerialized]
        public readonly View OwnerView;

        [SerializeField]
        public ValueConverterContext ValueConverterContext;

        [SerializeField]
        private List<string> _setFieldNames;

        private Dictionary<string, ViewFieldData> _fieldData;
        private HashSet<string> _setViewFields;
        private Dictionary<string, string> _expressionViewField;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public ViewFields(View ownerView)
        {
            OwnerView = ownerView;

            _setFieldNames = new List<string>();

            _fieldData = new Dictionary<string, ViewFieldData>();
            _setViewFields = new HashSet<string>();
            _expressionViewField = new Dictionary<string, string>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets boolean indicating if value has been set on view field.
        /// </summary>
        public bool IsSet<TField>(Expression<Func<TField>> expression)
        {
            return IsSet(GetMappedFieldPath(expression));
        }

        /// <summary>
        /// Gets boolean indicating if value has been set on view field.
        /// </summary>
        public bool IsSet(string fieldPath)
        {
            // get view field data
            var viewFieldData = GetData(fieldPath);
            if (viewFieldData == null)
            {
                Debug.LogError(String.Format(
                    "[MarkLight] {0}: Unable to get set-value from view field \"{1}\". View field not found.",
                    OwnerView.GameObjectName, fieldPath));
                return false;
            }

            return viewFieldData.IsSet();
        }

        /// <summary>
        /// Sets view field is-set indicator.
        /// </summary>
        public void SetIsSet(string fieldPath)
        {
            // get view field data
            var viewFieldData = GetData(fieldPath);
            if (viewFieldData == null)
            {
                Debug.LogError(String.Format(
                    "[MarkLight] {0}: Unable to set is-set indicator on view field \"{1}\". View field not found.",
                    OwnerView.GameObjectName, fieldPath));
                return;
            }

            viewFieldData.SetIsSet();
        }

        /// <summary>
        /// Gets value from view field.
        /// </summary>
        public object GetValue(string fieldPath)
        {
            bool hasValue;
            return GetValue(fieldPath, out hasValue);
        }

        /// <summary>
        /// Gets value from view field.
        /// </summary>
        public object GetValue(string fieldPath, out bool hasValue)
        {
            // get view field data
            var viewFieldData = GetData(fieldPath);
            if (viewFieldData == null)
            {
                hasValue = false;
                Debug.LogError(String.Format(
                    "[MarkLight] {0}: Unable to get value from view field \"{1}\". View field not found.",
                    OwnerView.GameObjectName, fieldPath));
                return null;
            }

            try
            {
                return viewFieldData.GetValue(out hasValue);
            }
            catch (Exception e)
            {
                hasValue = false;
                Debug.LogError(String.Format(
                    "[MarkLight] {0}: Unable to get value from view field \"{1}\". Exception thrown: {2}",
                    OwnerView.GameObjectName, fieldPath, Utils.GetError(e)));
                return null;
            }
        }

        /// <summary>
        /// Gets value from view field.
        /// </summary>
        public object GetValue<TField>(Expression<Func<TField>> expression)
        {
            return GetValue(GetMappedFieldPath(expression));
        }

        /// <summary>
        /// Sets view value.
        /// </summary>
        public object SetValue(string fieldPath, object value)
        {
            return SetValue(fieldPath, new ViewFieldValue(value));
        }

        /// <summary>
        /// Sets view field value.
        /// </summary>
        /// <param name="fieldPath">View field path.</param>
        /// <param name="inValue">Value context.</param>
        /// <returns>Returns the converted value set.</returns>
        public object SetValue(string fieldPath, ViewFieldValue inValue)
        {
            inValue.ConverterContext = inValue.ConverterContext ?? ValueConverterContext;

            // Debug.Log(String.Format("{0}: {1} = {2}", OwnerView.GameObjectName, fieldPath, value));

            // get view field data
            var fieldData = GetData(fieldPath);
            if (fieldData == null)
            {
                if (!inValue.SuppressAssignErrors)
                {
                    Debug.LogError(String.Format(
                        "[MarkLight] {0}: Unable to assign value \"{1}\" to view field \"{2}\". View field not found.",
                        OwnerView.GameObjectName, inValue.Value, fieldPath));
                }
                return null;
            }

            // if default state set default state value
            if (OwnerView.States.IsDefaultState && inValue.UpdateDefaultState)
            {
                var defaultValue = OwnerView.States.Get(View.DefaultStateName, fieldPath);
                if (defaultValue != null)
                    defaultValue.SetDefaultValue(inValue.Value, fieldData.ValueConverter.ConvertToString(inValue.Value));
            }

            // set view field value
            try
            {
                return fieldData.SetValue(inValue);
            }
            catch (Exception e)
            {
                if (!inValue.SuppressAssignErrors)
                {
                    Debug.LogError(String.Format(
                        "[MarkLight] {0}: Unable to assign value \"{1}\" to view field \"{2}\". Exception thrown: {3}",
                        OwnerView.GameObjectName, inValue.Value, fieldPath, Utils.GetError(e)));
                }
                return null;
            }
        }

        /// <summary>
        /// Sets the value of a field utilizing the binding and change tracking system.
        /// </summary>
        public object SetValue<TField>(Expression<Func<TField>> expression, ViewFieldValue inValue)
        {
            return SetValue(GetMappedFieldPath(expression), inValue);
        }

        /// <summary>
        /// Gets view field data.
        /// </summary>
        public ViewFieldData GetData(string fieldPath, int depth = int.MaxValue)
        {
            // get mapped view field
            var viewTypeData = ViewData.GetViewTypeData(OwnerView.ViewTypeName);
            var mappedField = viewTypeData.GetMappedFieldPath(fieldPath);

            if (_fieldData == null)
                _fieldData = new Dictionary<string, ViewFieldData>();

            // is there data for this field?
            var fieldData = _fieldData.Get(mappedField);
            if (fieldData == null)
            {
                // no. create new field data
                fieldData = ViewFieldData.FromPath(OwnerView, mappedField);
                if (fieldData != null)
                {
                    _fieldData.Add(mappedField, fieldData);
                }
            }

            // are we the owner of this field?
            if (fieldData != null && fieldData.IsMapped && depth > 0)
            {
                // no. go deeper if we can
                var targetView = fieldData.GetTargetView();
                if (targetView != null)
                {
                    fieldData = targetView.Fields.GetData(fieldData.TargetPath, --depth);
                }
            }

            return fieldData;
        }

        /// <summary>
        /// Notifies all value observers that are dependent on the specified field. E.g. when field "Name" changes,
        /// value observers on "Name.FirstName" and "Name.LastName" are notified in this method.
        /// </summary>
        public void NotifyDependentValueObservers(string fieldPath,
                                                  bool includeViewField = false, bool includeMapped = true)
        {
            foreach (var viewFieldData in _fieldData.Values)
            {
                if (!includeMapped && viewFieldData.IsMapped)
                    continue;

                var isMatch = includeViewField && viewFieldData.Path == fieldPath ||
                              viewFieldData.HasDependency(fieldPath);

                if (!isMatch)
                    continue;

                if (viewFieldData.IsMapped)
                {
                    viewFieldData.TargetView.Fields.NotifyDependentValueObservers(
                        viewFieldData.TargetPath, includeViewField);
                }
                else
                {
                    viewFieldData.NotifyValueObservers(new HashSet<ViewFieldData>());
                }
            }
        }

        /// <summary>
        /// Called once to queue all change handlers. Called in reverse breadth-first order.
        /// </summary>
        public void QueueAllChangeHandlers()
        {
            var fieldDataList = new List<ViewFieldData>(_fieldData.Values);
            foreach (var fieldData in fieldDataList)
            {
                fieldData.NotifyChangeHandlerValueObservers(new HashSet<ViewFieldData>());
            }
        }

        /// <summary>
        /// Sets view field change handler.
        /// </summary>
        internal void SetChangeHandler(ViewFieldChangeHandler handler)
        {
            // get view field data for change handler
            var viewFieldData = GetData(handler.ViewField);
            if (viewFieldData == null)
            {
                return;
            }

            // create change handler observer
            var observer = new ChangeHandlerValueObserver(
                                OwnerView, handler.ChangeHandlerName, handler.TriggerImmediately);

            if (observer.IsValid)
            {
                viewFieldData.RegisterValueObserver(observer);
            }
            else
            {
                Debug.LogError(String.Format(
                    "[MarkLight] {0}: Unable to assign view change handler \"{1}()\" for view field \"{2}\". "+
                    "Change handler method not found.",
                    OwnerView.GameObjectName, handler.ChangeHandlerName, handler.ViewField));
            }
        }

        /// <summary>
        /// Adds view field path to list of set view fields.
        /// </summary>
        internal void AddIsSetField(string fieldPath)
        {
            if (!_setFieldNames.Contains(fieldPath))
            {
                _setFieldNames.Add(fieldPath);
            }

            if (_setViewFields != null)
            {
                _setViewFields.Add(fieldPath);
            }
        }

        /// <summary>
        /// Gets bool indicating if set field is in list of set fields.
        /// </summary>
        internal bool GetIsSetFieldValue(string fieldPath)
        {
            if (_setViewFields != null)
            {
                return _setViewFields.Contains(fieldPath);
            }

            return false;
        }


        /// <summary>
        /// Initializes internal values to default values. Called once before InitializeInternal().
        /// Called in depth-first order.
        /// </summary>
        internal void InitializeInternalDefaultValues() {
            _fieldData = new Dictionary<string, ViewFieldData>();
            _setViewFields = new HashSet<string>();
            _expressionViewField = new Dictionary<string, string>();
        }

        /// <summary>
        /// Initializes the field manager internally.
        /// Called once before Initialize(). Called in depth-first order.
        /// </summary>
        internal void InitializeInternal() {
            // initialize set-fields
            foreach (var setField in _setFieldNames)
            {
                _setViewFields.Add(setField);
            }
        }


        /// <summary>
        /// Gets mapped view field from expression.
        /// </summary>
        protected string GetMappedFieldPath<TField>(Expression<Func<TField>> expression)
        {
            // get mapped view field
            var expressionString = expression.ToString();
            var mappedViewField = _expressionViewField.Get(expressionString);
            if (mappedViewField != null)
                return mappedViewField;

            // add expression
            mappedViewField =
                expressionString.Substring(expressionString.IndexOf(").", StringComparison.Ordinal) + 2);
            _expressionViewField.Add(expressionString, mappedViewField);

            return mappedViewField;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get readonly collection of view field data.
        /// </summary>
        public ICollection<ViewFieldData> Data
        {
            get { return _fieldData.Values; }
        }

        /// <summary>
        /// Gets view field data of the view.
        /// </summary>
        public IDictionary<string, ViewFieldData> FieldDataDictionary
        {
            get { return _fieldData; }
        }

        #endregion
    }
}