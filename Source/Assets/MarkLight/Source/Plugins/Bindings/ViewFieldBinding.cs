using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace MarkLight
{
    /// <summary>
    /// Contains data about a view field binding.
    /// </summary>
    [Serializable]
    public class ViewFieldBinding
    {
        #region Fields

        public static readonly Regex BindingRegex = new Regex(
            @"{(?<field>[A-Za-z0-9_#!=@^:\.\[\]]+)(?<format>:[^}]+)?}");

        public static readonly Regex TransformBindingRegex = new Regex(
            @"(?<function>\$[A-Za-z0-9_]+)\((?<params>[A-Za-z0-9_#!=@^:\{\}\.\, ]+)\)");

        [SerializeField]
        private string _bindingString;

        [SerializeField]
        private string _fieldPath;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor for serialization.
        /// </summary>
        protected ViewFieldBinding() {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ViewFieldBinding(string fieldPath, string bindingString)
        {
            _bindingString = bindingString;
            _fieldPath = fieldPath;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Checks if a view field value contains bindings.
        /// </summary>
        public static bool HasBindings(string value)
        {
            var startBracketIndex = value.IndexOf('{');
            if (startBracketIndex >= 0)
            {
                return value.IndexOf('}') > startBracketIndex;
            }
            return false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get binding string.
        /// </summary>
        public string BindingString
        {
            get { return _bindingString; }
        }

        /// <summary>
        /// Get the name of the view field.
        /// </summary>
        public string FieldPath
        {
            get { return _fieldPath; }
        }

        #endregion
    }
}
