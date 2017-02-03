﻿using System;
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
            @"{(?<field>[A-Za-z0-9_#!=@\.\[\]]+)(?<format>:[^}]+)?}");

        public static readonly Regex TransformBindingRegex = new Regex(
            @"(?<function>\$[A-Za-z0-9_]+)\((?<params>[A-Za-z0-9_#!=@\{\}\.\, ]+)\)");

        [SerializeField]
        private string _bindingString;

        [SerializeField]
        private string _fieldName;

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
        public ViewFieldBinding(string bindingString, string viewFieldName)
        {
            _bindingString = bindingString;
            _fieldName = viewFieldName;
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
                return value.IndexOf('}') > startBracketIndex;  //BindingRegex.IsMatch(value) || TransformBindingRegex.IsMatch(value);
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
        public string FieldName
        {
            get { return _fieldName; }
        }

        #endregion
    }
}
