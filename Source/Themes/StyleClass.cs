using System;
using System.Collections.Generic;
using UnityEngine;

namespace Marklight.Themes
{
    [Serializable]
    public class StyleClass
    {
        #region Private Fields

        private static readonly char[] StyleSeperator = new char[] { ' ' };

        [SerializeField]
        private string _className;

        [NonSerialized]
        private string[] _classNames;

        [NonSerialized]
        private string _selector;

        #endregion

        #region Constructors

        public StyleClass()
        {
        }

        public StyleClass(string className)
        {
            _className = SortClassNames(className ?? "");
        }

        #endregion

        #region Methods

        public override int GetHashCode()
        {
            return _className.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var other = obj as StyleClass;
            if (other == null)
                return false;

            return other._className == _className;
        }

        /// <summary>
        /// Sort a space delimited string of style class names alphabetically.
        /// </summary>
        public static string SortClassNames(string className)
        {

            if (string.IsNullOrEmpty(className))
                return className;

            var names = new List<string>(className.Split(StyleSeperator, StringSplitOptions.RemoveEmptyEntries));
            if (names.Count == 1)
                return names[0];

            names.Sort(StringComparer.Ordinal);
            return string.Join(" ", names.ToArray());
        }

        #endregion

        #region Property

        public bool IsSet
        {
            get { return !string.IsNullOrEmpty(_className); }
        }

        public string ClassName
        {
            get { return _className; }
        }

        public string[] ClassNames
        {
            get { return _classNames ?? (_classNames = _className.Split(' ')); }
        }

        public string Selector
        {
            get { return _selector ?? (_selector = "." + string.Join(".", ClassNames)); }
        }

        #endregion
    }
}