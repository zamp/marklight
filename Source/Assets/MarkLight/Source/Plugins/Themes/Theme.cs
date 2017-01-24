using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MarkLight;
using UnityEngine;

namespace Marklight.Themes
{
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

        public Theme() {
        }

        public Theme(string name, string baseDirectory, Vector3 unitSize,
                     bool isBaseDirectorySet, bool isUnitSizeSet, List<Style> styles) {

            _name = name;
            _baseDirectory = baseDirectory;
            _unitSize = unitSize;
            _isBaseDirectorySet = isBaseDirectorySet;
            _isUnitSizeSet = isUnitSizeSet;
            _styles = styles;
            Style.Sort(styles);
        }

        #endregion

        #region Methods

        public void ApplyTo(View view, ValueConverterContext context) {

            var style = GetStyle(view);
            if (style == null)
                return;

            context = new ValueConverterContext(context);

            if (_isBaseDirectorySet)
                context.BaseDirectory = _baseDirectory;

            if (_isUnitSizeSet)
                context.UnitSize = _unitSize;

            style.ApplyTo(view, context);
        }

        public Style GetStyle(View view)
        {
            return GetStyle(view.ViewXumlName, view.Id, view.Style);
        }

        public Style GetStyle(string elementName, string id, string className)
        {
            BuildStyleLookup();
            Style style;

            // see if style is already cached
            var newStyle = new Style(elementName, id, className);
            if (_styleLookup.TryGetValue(newStyle.Selector, out style))
                return style;

            // find all styles that are applicable
            var styles = GetMatchingStyles(elementName, id, className, new StyleSet());

            if (styles.Count == 0)
                return null;

            if (styles.Count == 1)
                return styles[0];

            // combine properties into new style
            var properties = new StylePropertySet();
            foreach (var s in styles)
            {
                AddProperties(s, properties);
            }

            Style existing;
            if (_styleLookup.TryGetValue(newStyle.Selector, out existing))
            {
                AddProperties(properties, existing.Properties);
                return existing;
            }

            newStyle.Properties.AddRange(properties);
            _styleLookup[newStyle.Selector] = newStyle;
            return newStyle;
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

        private StyleSet GetMatchingStyles(string elementName, string id, string className, StyleSet output) {

            // Element
            var elemStyles = GetStyleCollection(elementName, _byElement);
            if (elemStyles != null)
            {
                GetMatchingStyles(elementName, id, className, elemStyles, output);
            }

            // Id
            if (!string.IsNullOrEmpty(id))
            {
                var idStyles = GetStyleCollection(id, _byId);
                if (idStyles != null)
                {
                    GetMatchingStyles(elementName, id, className, idStyles, output);
                }
            }

            // Classes
            if (!string.IsNullOrEmpty(className)) {
                var styleClass = new StyleClass(className);

                foreach (var name in styleClass.ClassNames)
                {
                    var classStyles = GetStyleCollection(name, _byClass);
                    if (classStyles != null)
                        GetMatchingStyles(elementName, id, className, classStyles, output);
                }
            }

            return output;
        }

        private static void GetMatchingStyles(string elementName, string id, string className, StyleCollection styles, StyleSet output) {
            foreach (var style in styles)
            {
                if (style.IsApplicable(elementName, id, className))
                {
                    output.Add(style);
                }
            }
        }

        private void BuildStyleLookup() {
            if (_styleLookup != null)
                return;

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
                return;
            }

            if (!string.IsNullOrEmpty(style.Id))
            {
                var idStyles = GetStyleCollection(style.Id, _byId, true);
                if (!idStyles.Contains(style))
                    idStyles.Add(style);
                return;
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

        public string Name
        {
            get { return _name; }
        }

        public string BaseDirectory
        {
            get { return _baseDirectory; }
        }

        public Vector3 UnitSize
        {
            get { return _unitSize; }
        }

        public bool IsBaseDirectorySet
        {
            get { return _isBaseDirectorySet; }
        }

        public bool IsUnitSizeSet
        {
            get { return _isUnitSizeSet; }
        }

        #endregion

        #region Private Classes

        private class StyleSet : KeyedCollection<Style, Style>
        {
            protected override Style GetKeyForItem(Style item) {
                return item;
            }
        }

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