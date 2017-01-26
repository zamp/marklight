using System;
using System.Collections.Generic;
using UnityEngine;

namespace Marklight.Themes
{
    /// <summary>
    /// Serializable style data.
    /// </summary>
    [Serializable]
    public class StyleData
    {
        #region Private Fields

        [SerializeField]
        private int _index;

        [SerializeField]
        private int _parentIndex;

        [SerializeField]
        private string _elementName;

        [SerializeField]
        private string _id;

        [SerializeField]
        private string _className;

        [SerializeField]
        private StyleCombinatorType _combinatorType;

        [SerializeField]
        private string _basedOn;

        [SerializeField]
        private List<StyleProperty> _properties = new List<StyleProperty>(10);

        [NonSerialized]
        private Style _style;

        #endregion

        #region Constructors

        /// <summary>
        /// Serialization Constructor.
        /// </summary>
        protected StyleData()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="index">The styles array index position.</param>
        /// <param name="parentIndex">The array index position of the parent style. -1 if there is no parent.</param>
        /// <param name="elementName">The element name component of the style selector.</param>
        /// <param name="id">The id component of the style selector.</param>
        /// <param name="className">The class name component of the style selector.</param>
        /// <param name="combinatorType">The type of selector combinator.</param>
        /// <param name="basedOn">The style that is inherited.</param>
        /// <exception cref="InvalidOperationException">When index and parentIndex are the same.</exception>
        public StyleData(int index, int parentIndex,
                         string elementName, string id, string className,
                         StyleCombinatorType combinatorType, string basedOn = null) {

            if (index == parentIndex)
                throw new InvalidOperationException("Style cannot be a child of itself. " + index);

            _index = index;
            _parentIndex = parentIndex;
            _elementName = elementName ?? "";
            _id = id ?? "";
            _className = className ?? "";
            _combinatorType = combinatorType;
            _basedOn = basedOn ?? "";
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get the style instance for the theme.
        /// </summary>
        /// <param name="theme">The theme the style belongs to.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">When the wrong theme is used.</exception>
        public Style GetStyle(Theme theme) {

            if (_style == null)
            {
                _style = new Style(theme, this);
            }

            if (_style.Theme != theme)
                throw new InvalidOperationException("Incorrect theme.");

            return _style;
        }

        public override int GetHashCode()
        {
            return _index;
        }

        public override bool Equals(object obj)
        {
            var other = obj as StyleData;
            if (other == null)
                return false;

            return _index == other._index;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the styles array index position.
        /// </summary>
        public int Index
        {
            get { return _index; }
        }

        /// <summary>
        /// Get the array index position of the parent style. -1 if there is no parent.
        /// </summary>
        public int ParentIndex
        {
            get { return _parentIndex; }
        }

        /// <summary>
        /// Get the element name component of the style selector.
        /// </summary>
        public string ElementName
        {
            get { return _elementName; }
        }

        /// <summary>
        /// Get the ID component of the style selector.
        /// </summary>
        public string Id
        {
            get { return _id; }
        }

        /// <summary>
        /// Get the class name component of the style selector.
        /// </summary>
        public string ClassName
        {
            get { return _className; }
        }

        /// <summary>
        /// Get the style selector combinator type.
        /// </summary>
        public StyleCombinatorType CombinatorType
        {
            get { return _combinatorType; }
        }

        /// <summary>
        /// Get the name of the style that this style is based on.
        /// </summary>
        public string BasedOn
        {
            get { return _basedOn; }
        }

        /// <summary>
        /// Get the properties list.
        /// </summary>
        public List<StyleProperty> Properties
        {
            get { return _properties; }
        }

        #endregion
    }
}