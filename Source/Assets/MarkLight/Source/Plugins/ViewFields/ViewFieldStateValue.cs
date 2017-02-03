using System;
using UnityEngine;

namespace MarkLight
{
    /// <summary>
    /// Contains data about a view field state value.
    /// </summary>
    [Serializable]
    public class ViewFieldStateValue
    {
        #region Fields

        [SerializeField]
        private string _path;

        [SerializeField]
        private string _state;

        [SerializeField]
        private string _stringValue;

        [SerializeField]
        private ValueConverterContext _converterContext;

        [SerializeField]
        private bool _isDefaultValueSet = true;

        [NonSerialized]
        private object _cachedValue;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for serialization.
        /// </summary>
        protected ViewFieldStateValue() {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ViewFieldStateValue(string path, string state, string stringValue, ValueConverterContext context,
            bool isDefaultValueSet) {

            _path = path;
            _state = state;
            _stringValue = stringValue;
            _converterContext = context;
            _isDefaultValueSet = isDefaultValueSet;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets value of field.
        /// </summary>
        public void SetDefaultValue(object value, string stringValue)
        {
            _cachedValue = value;
            _stringValue = stringValue;
            _isDefaultValueSet = true;
        }

        /// <summary>
        /// Sets value of field.
        /// </summary>
        public void SetCachedValue(object value)
        {
            _cachedValue = value;
        }

        /// <summary>
        /// Sets value of field and converter context.
        /// </summary>
        public void SetValue(string stringValue, ValueConverterContext context) {
            _stringValue = stringValue;
            _converterContext = context;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the view field path.
        /// </summary>
        public string Path
        {
            get { return _path; }
        }

        /// <summary>
        /// Get the view field state name.
        /// </summary>
        public string State
        {
            get { return _state; }
        }

        /// <summary>
        /// Get the view field value.
        /// </summary>
        public object Value
        {
            get { return _cachedValue != null ? _cachedValue : _stringValue; }
        }

        /// <summary>
        /// Get the converter context.
        /// </summary>
        public ValueConverterContext ConverterContext
        {
            get { return _converterContext; }
        }

        /// <summary>
        /// Determine if the default value is set.
        /// </summary>
        public bool IsDefaultValueSet
        {
            get { return _isDefaultValueSet; }
        }

        #endregion
    }
}
