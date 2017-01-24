using System;
using System.Collections;
using System.Collections.Generic;
using MarkLight;
using UnityEngine;

namespace Marklight.Themes
{
    [Serializable]
    public class Style : IEnumerable<StyleProperty>
    {
        #region Private Fields

        [SerializeField]
        private string _elementName;

        [SerializeField]
        private string _id;

        [SerializeField]
        private string _className;

        [SerializeField]
        private string _basedOn;

        [SerializeField]
        private StyleSelectorType _selectorType;

        [SerializeField]
        private List<StyleProperty> _properties = new List<StyleProperty>(10);

        [SerializeField]
        private List<string> _childSelectors;

        [NonSerialized]
        private string _selector;

        [NonSerialized]
        private string[] _selectors;

        [NonSerialized]
        private StyleClass _class;

        #endregion

        #region Constructors

        public Style()
        {
        }

        public Style(string elementName, string id, string className, string basedOn = null)
        {
            _elementName = elementName ?? "";
            _id = id ?? "";
            _className = className ?? "";
            _basedOn = basedOn ?? "";

            if (_elementName != "")
                _selectorType |= StyleSelectorType.Element;

            if (_id != "")
                _selectorType |= StyleSelectorType.Id;

            if (className != "")
                _selectorType |= StyleSelectorType.Class;
        }

        #endregion

        #region Methods

        public void ApplyTo(View view, ValueConverterContext context)
        {
            foreach (var property in _properties)
            {
                property.ApplyTo(view, new ValueConverterContext(context));
            }
        }

        public bool IsApplicable(View view)
        {
            return IsApplicable(view.ViewXumlName, view.Id, view.Style);
        }

        public bool IsApplicable(string elementName, string id, string className)
        {
            if (!string.IsNullOrEmpty(_elementName) && elementName != _elementName)
                return false;

            if (!string.IsNullOrEmpty(_id) && id != _id)
                return false;

            if (!StyleClass.IsSet)
                return true;

            if (string.IsNullOrEmpty(className))
                return false;

            var viewStyle = new StyleClass(className);

            foreach (var ownClassName in StyleClass.ClassNames)
            {
                var hasOwnClass = false;
                foreach (var name in viewStyle.ClassNames)
                {
                    if (name != ownClassName)
                        continue;

                    hasOwnClass = true;
                    break;
                }

                if (!hasOwnClass)
                    return false;
            }

            return true;
        }

        public IEnumerator<StyleProperty> GetEnumerator()
        {
            return _properties.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override int GetHashCode()
        {
            var result = 128;
            if (!string.IsNullOrEmpty(_elementName))
                result ^= _elementName.GetHashCode();
            if (!string.IsNullOrEmpty(_id))
                result ^= _id.GetHashCode();
            if (StyleClass.IsSet)
                result ^= StyleClass.GetHashCode();
            return result;
        }

        public override bool Equals(object obj)
        {
            var other = obj as Style;
            if (other == null)
                return false;

            if (other._elementName != _elementName || other._id != _id)
                return false;

            if (other._className == _className)
                return true;

            if (other.StyleClass.IsSet && StyleClass.IsSet)
                return other.StyleClass.ClassName == StyleClass.ClassName;

            return false;
        }

        /// <summary>
        /// Sort a list of styles by selector type. Elements at the top, Id's next, class names at the bottom.
        /// </summary>
        public static void Sort(List<Style> styles)
        {
            styles.Sort((style1, style2) =>
            {
                if (style1.SelectorType == style2.SelectorType)
                    return 0;

                if (style1.SelectorType.HasFlag(StyleSelectorType.Element))
                    return -1;

                if (style1.SelectorType.HasFlag(StyleSelectorType.Id))
                    return style2.SelectorType.HasFlag(StyleSelectorType.Element)
                        ? 1
                        : -1;

                if (style1.SelectorType.HasFlag(StyleSelectorType.Class))
                    return 1;

                if (style1.SelectorType == StyleSelectorType.None)
                    return 1;

                throw new ArgumentOutOfRangeException();
            });
        }

        #endregion

        #region Properties

        public string[] Selectors
        {
            get
            {
                if (_selectors != null)
                    return _selectors;

                var list = new List<string>();
                if (!string.IsNullOrEmpty(_className))
                {
                    var classNames = StyleClass.ClassNames;
                    foreach (var className in classNames)
                    {
                        var selector = "";

                        if (!string.IsNullOrEmpty(_elementName))
                            selector += _elementName;

                        if (!string.IsNullOrEmpty(_id))
                            selector += '#' + _id;

                        selector += '.' + className;

                        list.Add(selector);
                    }
                }
                else
                {
                    var selector = "";

                    if (!string.IsNullOrEmpty(_elementName))
                        selector += _elementName;

                    if (!string.IsNullOrEmpty(_id))
                        selector += '#' + _id;

                    list.Add(selector);
                }

                list.Sort(StringComparer.Ordinal);
                _selectors = list.ToArray();
                return _selectors;
            }
        }

        public string Selector
        {
            get
            {
                if (_selector != null)
                    return _selector;

                _selector = string.Join(",", Selectors);

                return _selector;
            }
        }

        public string ElementName
        {
            get { return _elementName; }
        }

        public string Id
        {
            get { return _id; }
        }

        public StyleClass StyleClass
        {
            get { return _class ?? (_class = new StyleClass(_className)); }
        }

        public string BasedOn
        {
            get { return _basedOn; }
        }

        public StyleSelectorType SelectorType
        {
            get { return _selectorType; }
        }

        public List<StyleProperty> Properties
        {
            get { return _properties; }
        }

        public List<string> ChildSelectors
        {
            get { return _childSelectors ?? (_childSelectors = new List<string>(0)); }
        }

        #endregion
    }
}