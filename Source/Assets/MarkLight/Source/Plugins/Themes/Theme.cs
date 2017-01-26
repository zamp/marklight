using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MarkLight;
using UnityEngine;

namespace Marklight.Themes
{
    /// <summary>
    /// Serializable theme data.
    /// </summary>
    [Serializable]
    public class Theme : IEnumerable<Style>
    {
        #region Private Fields

        [SerializeField]
        private string _name;

        [SerializeField]
        private string _baseDirectory;

        [SerializeField]
        private Vector3 _unitSize;

        [SerializeField]
        private bool _isBaseDirectorySet;

        [SerializeField]
        private bool _isUnitSizeSet;

        [SerializeField]
        private StyleData[] _styleData;

        [NonSerialized]
        private List<Style> _styles;

        [NonSerialized]
        private Dictionary<string, Style> _styleLookup;

        [NonSerialized]
        private StyleCollectionSet _byElement;

        [NonSerialized]
        private StyleCollectionSet _byId;

        [NonSerialized]
        private StyleCollectionSet _byClass;

        #endregion

        #region Constructor

        /// <summary>
        /// Serialization constructor.
        /// </summary>
        protected Theme() {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">The name of the theme.</param>
        /// <param name="baseDirectory">Base directory of the theme.</param>
        /// <param name="unitSize">The themes unit size.</param>
        /// <param name="isBaseDirectorySet">True if baseDirectory has a value.</param>
        /// <param name="isUnitSizeSet">True if unitSize has a value.</param>
        /// <param name="styles">Array of the themes style data.</param>
        public Theme(string name, string baseDirectory, Vector3 unitSize,
                     bool isBaseDirectorySet, bool isUnitSizeSet, StyleData[] styles) {

            _name = name;
            _baseDirectory = baseDirectory;
            _unitSize = unitSize;
            _isBaseDirectorySet = isBaseDirectorySet;
            _isUnitSizeSet = isUnitSizeSet;
            _styleData = styles;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get style data by array index position.
        /// </summary>
        /// <param name="index">The index position of the style data.</param>
        /// <returns>The style data.</returns>
        public StyleData GetStyleData(int index) {
            return _styleData[index];
        }

        /// <summary>
        /// Get all styles that are direct children of the specified parent index.
        /// </summary>
        /// <param name="parentIndex">The index position of the parent style.</param>
        /// <param name="output">A collection to add the results to.</param>
        /// <returns>The output collection.</returns>
        public T GetStyleChildData<T>(int parentIndex, T output) where T: ICollection<StyleData> {
            foreach (var data in _styleData)
            {
                if (data.ParentIndex == parentIndex)
                    output.Add(data);
            }
            return output;
        }

        /// <summary>
        /// Apply the theme to a view.
        /// </summary>
        /// <param name="view">The view to apply to.</param>
        /// <param name="filterMode">The mode used to filter styles applied.</param>
        /// <param name="context">The converter context.</param>
        public void ApplyTo(View view, StyleFilterMode filterMode, ValueConverterContext context) {

            var style = GetStyle(view, filterMode);
            if (style == null)
                return;

            context = new ValueConverterContext(context);

            if (_isBaseDirectorySet)
                context.BaseDirectory = _baseDirectory;

            if (_isUnitSizeSet)
                context.UnitSize = _unitSize;

            style.ApplyTo(view, context);
        }

        /// <summary>
        /// Get the style that is applicable to a view.
        /// </summary>
        /// <param name="view">The view to test.</param>
        /// <param name="mode">The mode used to filter styles.</param>
        /// <returns>Style or null if not found.</returns>
        public Style GetStyle(View view, StyleFilterMode mode)
        {
            return GetStyle(view.ViewXumlName, view.Id, view.Style, mode, view.LayoutParent);
        }

        /// <summary>
        /// Get the style that is applicable to the specified view selector components and parameters.
        /// </summary>
        /// <param name="elementName">The element name component of the style selector.</param>
        /// <param name="id">The ID component of the style selector.</param>
        /// <param name="className">The class name component of the style selector.</param>
        /// <param name="mode">The mode used to filter styles.</param>
        /// <param name="layoutParent">The view to consider as the layout parent while finding styles.</param>
        /// <returns>Style or null if not found.</returns>
        public Style GetStyle(string elementName, string id, string className, StyleFilterMode mode, View layoutParent)
        {
            BuildStyleLookup();

            // find all styles that are applicable
            var styles = GetMatchingStyles(elementName, id, className, mode, layoutParent, new List<Style>(3));

            if (styles.Count == 0)
                return null;

            if (styles.Count == 1)
                return styles[0];

            Style.Sort(styles);

            // combine properties into new style
            var properties = new StylePropertySet();
            foreach (var s in styles)
            {
                AddProperties(s, properties);
            }

            var selector = new StyleSelector(elementName, id, className, StyleCombinatorType.None);

            Style existing;
            if (_styleLookup.TryGetValue(selector.Selector, out existing))
            {
                AddProperties(properties, existing.Properties);
                return existing;
            }

            var composite = new Style(this, selector);
            composite.Properties.AddRange(properties);
            _styleLookup[composite.Selector] = composite;

            return composite;
        }

        public IEnumerator<Style> GetEnumerator()
        {
            return _styles.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void AddProperties<T>(Style style, T output) where T : ICollection<StyleProperty>
        {

            if (!string.IsNullOrEmpty(style.BasedOn))
            {
                // add properties from BasedOn style
                Style basedOnStyle;
                if (_styleLookup.TryGetValue((style.ElementName ?? "") + '.' + style.BasedOn, out basedOnStyle))
                {
                    AddProperties(basedOnStyle.Properties, output);
                }
            }

            // add properties
            AddProperties(style.Properties, output);
        }

        private void AddProperties<T>(IEnumerable<StyleProperty> properties, T output)
            where T : ICollection<StyleProperty>
        {
            foreach (var property in properties)
            {
                // remove existing property first
                output.Remove(property);

                output.Add(property);
            }
        }

        private List<Style> GetMatchingStyles(string elementName, string id, string className, StyleFilterMode mode, View layoutParent, List<Style> output) {

            // Element
            var elemStyles = GetStyleCollection(elementName, _byElement);
            if (elemStyles != null)
            {
                GetMatchingStyles(elementName, id, className, mode, elemStyles, layoutParent, output);
            }

            // Id
            if (!string.IsNullOrEmpty(id))
            {
                var idStyles = GetStyleCollection(id, _byId);
                if (idStyles != null)
                {
                    GetMatchingStyles(elementName, id, className, mode, idStyles, layoutParent, output);
                }
            }

            // Classes
            if (!string.IsNullOrEmpty(className)) {
                var styleClass = new StyleClass(className);

                foreach (var name in styleClass.ClassNames)
                {
                    var classStyles = GetStyleCollection(name, _byClass);
                    if (classStyles != null)
                        GetMatchingStyles(elementName, id, className, mode, classStyles, layoutParent, output);
                }
            }

            return output;
        }

        private static void GetMatchingStyles(string elementName, string id, string className,
                                              StyleFilterMode mode,
                                              StyleCollection styles, View layoutParent, List<Style> output) {
            foreach (var style in styles)
            {
                if (mode == StyleFilterMode.ChildOnly && style.Parent == null)
                    continue;

                if (mode == StyleFilterMode.RootOnly && style.Parent != null)
                    continue;

                if (output.Contains(style) || !style.IsApplicable(elementName, id, className, layoutParent))
                    continue;

                output.Add(style);
            }
        }

        private void BuildStyleLookup() {

            if (_styleLookup != null)
                return;

            _styles = new List<Style>(50);
            foreach (var data in _styleData)
            {
                if (data.ParentIndex < 0)
                    _styles.Add(data.GetStyle(this));
            }

            Style.Sort(_styles);

            _styleLookup =  new Dictionary<string, Style>(_styles.Count + 1);
            _byElement = new StyleCollectionSet();
            _byClass = new StyleCollectionSet();
            _byId = new StyleCollectionSet();

            foreach (var style in _styles)
            {
                AddStyleToLookups(style);
            }
        }

        private void AddStyleToLookups(Style style) {

            if (!string.IsNullOrEmpty(style.ElementName))
            {
                var elemStyles = GetStyleCollection(style.ElementName, _byElement, true);
                if (!elemStyles.Contains(style))
                    elemStyles.Add(style);
            }

            if (!string.IsNullOrEmpty(style.Id))
            {
                var idStyles = GetStyleCollection(style.Id, _byId, true);
                if (!idStyles.Contains(style))
                    idStyles.Add(style);
            }

            if (style.StyleClass.IsSet)
            {
                foreach (var className in style.StyleClass.ClassNames)
                {
                    var classStyles = GetStyleCollection(className, _byClass, true);
                    if (!classStyles.Contains(style))
                        classStyles.Add(style);
                }
            }

            var children = style.Children;
            foreach (var child in children)
            {
                AddStyleToLookups(child);
            }
        }

        private static StyleCollection GetStyleCollection(string key, StyleCollectionSet set, bool guarantee = false) {

            if (set.Contains(key))
                return set[key];

            if (!guarantee)
                return null;

            var result = new StyleCollection(key);
            set.Add(result);
            return result;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the name of the theme.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Get the relative base directory of the theme files.
        /// </summary>
        public string BaseDirectory
        {
            get { return _baseDirectory; }
        }

        /// <summary>
        /// Get the unit size used by the theme.
        /// </summary>
        public Vector3 UnitSize
        {
            get { return _unitSize; }
        }

        /// <summary>
        /// Determine if the BaseDirectory property value is set.
        /// </summary>
        public bool IsBaseDirectorySet
        {
            get { return _isBaseDirectorySet; }
        }

        /// <summary>
        /// Determine if the UnitSize property value is set.
        /// </summary>
        public bool IsUnitSizeSet
        {
            get { return _isUnitSizeSet; }
        }

        #endregion

        #region Private Classes

        private class StylePropertySet : KeyedCollection<StyleProperty, StyleProperty>
        {
            protected override StyleProperty GetKeyForItem(StyleProperty item) {
                return item;
            }
        }

        private class StyleCollectionSet : KeyedCollection<string, StyleCollection>
        {
            protected override string GetKeyForItem(StyleCollection item) {
                return item.Selector;
            }
        }

        #endregion
    }
}