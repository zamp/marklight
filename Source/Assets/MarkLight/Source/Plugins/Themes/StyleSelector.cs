using System;
using System.Collections.Generic;
using System.Text;
using MarkLight;

namespace Marklight.Themes
{
    /// <summary>
    /// A style selector. Used to define what elements are applicable for a style.
    /// </summary>
    public class StyleSelector
    {
        #region Private Fields

        private readonly string _elementName;
        private readonly string _id;
        private readonly string _className;
        private readonly StyleSelector _parent;
        private readonly StyleSelectorType _selectorType;
        private readonly int _hash;

        private string _selector;
        private string[] _selectors;
        private string _localSelector;
        private string[] _localSelectors;
        private StyleClass _class;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="elementName">The element name component of the style selector.</param>
        /// <param name="id">The ID component of the style selector.</param>
        /// <param name="className">The class name component of the style selector.</param>
        /// <param name="parent">The parent style selector or null. Default is null.</param>
        public StyleSelector(string elementName, string id, string className, StyleSelector parent = null) {
            _elementName = elementName ?? "";
            _id = id ?? "";
            _className = className ?? "";
            _parent = parent;

            if (_elementName != "")
                _selectorType |= StyleSelectorType.Element;

            if (_id != "")
                _selectorType |= StyleSelectorType.Id;

            if (_className != "")
                _selectorType |= StyleSelectorType.Class;

            _hash = (_elementName != null ? _elementName.GetHashCode() : 128)
                    ^ (_id != null ? _id.GetHashCode() : 256);
        }

        /// <summary>
        /// Constructor that parses a selector string.
        /// </summary>
        public StyleSelector(string selector) {

            var mode = StyleSelectorType.Element;
            var buffer = new StringBuilder(selector.Length);
            var className = "";
            var id = "";
            var elementName = "";

            for (var i = 0; i < selector.Length; i++)
            {
                var ch = selector[i];
                if (" \r\n\t".IndexOf(ch) != -1)
                {
                    throw new Exception("White space not accepted in style selector. " + selector);
                }

                if (ch == '.' || ch == '#' || i == selector.Length - 1)
                {

                    if (i == selector.Length - 1)
                        buffer.Append(ch);

                    switch (mode)
                    {
                        case StyleSelectorType.Class:
                            className += (className.Length > 0 ? " " : "") + buffer;
                            break;
                        case StyleSelectorType.Id:
                            id += buffer.ToString();
                            break;
                        case StyleSelectorType.Element:
                            elementName = buffer.ToString();
                            break;
                    }

                    buffer.Length = 0;

                    switch (ch)
                    {
                        case '.':
                            mode = StyleSelectorType.Class;
                            break;
                        case '#':
                            mode = StyleSelectorType.Id;
                            break;
                    }
                    continue;
                }

                buffer.Append(ch);
            }

            _elementName = elementName;
            _id = id;
            _className = className;
            _parent = null;

            if (_elementName != "")
                _selectorType |= StyleSelectorType.Element;

            if (_id != "")
                _selectorType |= StyleSelectorType.Id;

            if (_className != "")
                _selectorType |= StyleSelectorType.Class;

            _hash = (_elementName != null ? _elementName.GetHashCode() : 128)
                    ^ (_id != null ? _id.GetHashCode() : 256);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Determine if the selector is applicable to a view.
        /// </summary>
        /// <param name="view">The view to test.</param>
        /// <returns>True if the selector is applicable to the style, otherwise false.</returns>
        public bool IsApplicable(View view) {
            return IsApplicable(view.ViewXumlName, view.Id, view.Style, view.LayoutParent);
        }

        /// <summary>
        /// Determine if the selector is applicable to the specified selector components.
        /// </summary>
        /// <param name="elementName">The element name component of the selector.</param>
        /// <param name="id">The ID component of the selector.</param>
        /// <param name="className">The class name component of the selector.</param>
        /// <param name="layoutParent">The view layout parent or null if none.</param>
        /// <returns>True if the selector is applicable to the specified selector components, otherwise false.</returns>
        public bool IsApplicable(string elementName, string id, string className, View layoutParent)
        {
            if (!string.IsNullOrEmpty(ElementName) && elementName != ElementName)
                return false;

            if (!string.IsNullOrEmpty(Id) && id != Id)
                return false;

            if (!StyleClass.IsSet)
                return IsParentApplicable(layoutParent);

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

            return IsParentApplicable(layoutParent);
        }

        public override int GetHashCode()
        {
            return _hash;
        }

        public override bool Equals(object obj)
        {
            var other = obj as StyleSelector;
            if (other == null)
                return false;

            if (!Equals(_parent, other._parent))
                return false;

            if (other.ElementName != ElementName || other.Id != Id)
                return false;

            if (other.StyleClass.IsSet && StyleClass.IsSet)
                return other.StyleClass.ClassName == StyleClass.ClassName;

            return false;
        }

        public override string ToString() {
            return Selector;
        }

        private bool IsParentApplicable(View layoutParent) {
            if (_parent == null || layoutParent == null)
                return true;

            var viewParent = layoutParent;
            while (viewParent != null)
            {
                if (_parent.IsApplicable(viewParent))
                    return true;

                viewParent = viewParent.LayoutParent;
            }
            return false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get an array of selectors. These selectors include any parent selectors.
        /// </summary>
        public string[] Selectors
        {
            get
            {
                if (_parent == null)
                    return LocalSelectors;

                if (_selectors != null)
                    return _selectors;

                var localSelectors = Selectors;
                var selectors = new List<string>(localSelectors.Length);
                foreach (var local in localSelectors)
                {
                    foreach (var parentSelector in _parent.Selectors)
                    {
                        selectors.Add(parentSelector + ' ' + local);
                    }
                }

                _selectors = selectors.ToArray();
                return _localSelectors;
            }
        }

        /// <summary>
        /// Get all selectors is a comma delimited string.
        /// </summary>
        public string Selector
        {
            get
            {
                if (_parent == null)
                    return LocalSelector;

                if (_selector != null)
                    return _selector;

                _selector = string.Join(",", Selectors);

                return _selector;
            }
        }

        /// <summary>
        /// Get an array of selectors that do not include parent selectors.
        /// </summary>
        public string[] LocalSelectors
        {
            get
            {
                if (_localSelectors != null)
                    return _localSelectors;

                var list = new List<string>();
                if (!string.IsNullOrEmpty(_className))
                {
                    var classNames = StyleClass.ClassNames;
                    foreach (var className in classNames)
                    {
                        var selector = "";

                        if (!string.IsNullOrEmpty(ElementName))
                            selector += ElementName;

                        if (!string.IsNullOrEmpty(Id))
                            selector += '#' + Id;

                        selector += '.' + className;

                        list.Add(selector);
                    }
                }
                else
                {
                    var selector = "";

                    if (!string.IsNullOrEmpty(ElementName))
                        selector += ElementName;

                    if (!string.IsNullOrEmpty(Id))
                        selector += '#' + Id;

                    list.Add(selector);
                }

                list.Sort(StringComparer.Ordinal);
                _localSelectors = list.ToArray();
                return _localSelectors;
            }
        }

        /// <summary>
        /// Get all local selectors as a comma delimited string.
        /// </summary>
        public string LocalSelector
        {
            get
            {
                if (_localSelector != null)
                    return _localSelector;

                _localSelector = string.Join(",", LocalSelectors);

                return _localSelector;
            }
        }

        /// <summary>
        /// Get the element name component of the selector.
        /// </summary>
        public string ElementName
        {
            get { return _elementName; }
        }

        /// <summary>
        /// Get the ID component of the selector.
        /// </summary>
        public string Id
        {
            get { return _id; }
        }

        /// <summary>
        /// Get the class name component of the selector.
        /// </summary>
        public string ClassName
        {
            get { return _className; }
        }

        /// <summary>
        /// Get style class data.
        /// </summary>
        public StyleClass StyleClass
        {
            get { return _class ?? (_class = new StyleClass(_className)); }
        }

        /// <summary>
        /// Get selector type flags.
        /// </summary>
        public StyleSelectorType SelectorType
        {
            get { return _selectorType; }
        }

        /// <summary>
        /// Get style selector parent or null if none.
        /// </summary>
        public StyleSelector Parent
        {
            get { return _parent; }
        }

        #endregion
    }
}