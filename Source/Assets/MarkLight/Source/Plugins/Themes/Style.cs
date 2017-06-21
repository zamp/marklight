using System;
using System.Collections.Generic;
using MarkLight;
using UnityEngine;

namespace Marklight.Themes
{
    /// <summary>
    /// Transient unserialized style for in memory style tree.
    /// </summary>
    public class Style : StyleSelector
    {
        #region Private Fields

        private static readonly Style[] EmptyStyleArray = new Style[0];

        private readonly int _index;
        private readonly Theme _theme;
        private readonly string _basedOn;
        private readonly List<StyleProperty> _properties;

        private IEnumerable<Style> _children;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="theme">The theme the style belongs to.</param>
        /// <param name="selector">A selector to copy from.</param>
        public Style(Theme theme, StyleSelector selector)
            : this(theme, selector.ElementName, selector.Id, selector.ClassName, selector.CombinatorType, null) {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="theme">The theme the style belongs to.</param>
        /// <param name="data">The style data to copy from.</param>
        public Style(Theme theme, StyleData data)
            : this(theme, data.ElementName, data.Id, data.ClassName, data.CombinatorType, data.BasedOn, data.Properties,
                GetParent(theme, data)) {

            _index = data.Index;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="theme">The theme the style belongs to.</param>
        /// <param name="elementName">The element name component of the style selector.</param>
        /// <param name="id">The ID component of the style selector.</param>
        /// <param name="className">The class name component of the style selector.</param>
        /// <param name="combinatorType">The selector combinator type.</param>
        /// <param name="basedOn">The style to inherit from.</param>
        /// <param name="properties">Properties to copy or null.</param>
        /// <param name="parent">The parent style or null.</param>
        public Style(Theme theme, string elementName, string id, string className,
                     StyleCombinatorType combinatorType, string basedOn,
                     IEnumerable<StyleProperty> properties = null, StyleSelector parent = null)
                     : base(elementName, id, className, combinatorType, parent) {

            _children = null;
            _theme = theme;
            _basedOn = basedOn ?? "";
            _properties = properties != null
                ? new List<StyleProperty>(properties)
                : new List<StyleProperty>(10);
            _index = -1;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Apply the style to a view.
        /// </summary>
        public void ApplyTo(View view, ValueConverterContext context)
        {
            foreach (var property in _properties)
            {
                property.ApplyTo(view, new ValueConverterContext(context));
            }
        }

        public override int GetHashCode()
        {
            return _index == -1 ? base.GetHashCode() : _index;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Style;
            return other != null && base.Equals(obj);
        }

        /// <summary>
        /// Sort a list of styles by selector type. Elements at the top, Id's next, class names at the bottom.
        /// </summary>
        public static void Sort(List<Style> styles)
        {
            styles.Sort((style1, style2) => style1.Specificity.CompareTo(style2.Specificity));
        }

        private static Style GetParent(Theme theme, StyleData data) {
            var parentData = data.ParentIndex >= 0
                ? theme.GetStyleData(data.ParentIndex)
                : null;

            return parentData != null
                ? parentData.GetStyle(theme)
                : null;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the theme the style belongs to.
        /// </summary>
        public Theme Theme
        {
            get { return _theme; }
        }

        /// <summary>
        /// Get the name of the style that this style is based on.
        /// </summary>
        public string BasedOn
        {
            get { return _basedOn; }
        }

        /// <summary>
        /// Get the styles property list.
        /// </summary>
        public List<StyleProperty> Properties
        {
            get { return _properties; }
        }

        /// <summary>
        /// Get child styles.
        /// </summary>
        public IEnumerable<Style> Children
        {
            get
            {
                if (_children != null)
                    return _children;

                if (_index >= 0)
                {
                    var childData = Theme.GetStyleChildData(_index, new List<StyleData>());
                    var children = new List<Style>(childData.Count);
                    foreach (var child in childData)
                    {
                        children.Add(child.GetStyle(Theme));
                    }
                    Sort(children);
                    _children = children;
                }
                else
                {
                    _children = EmptyStyleArray;
                }

                return _children;
            }
        }

        #endregion
    }
}