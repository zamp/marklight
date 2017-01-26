using System.Collections.Generic;
using System.Text;

namespace Marklight.Themes
{
    /// <summary>
    /// Parses CSS text into objects.
    /// </summary>
    public class CssParser
    {
        #region Private Fields

        private readonly string _css;
        private int _index;
        private ParseMode _mode;
        private readonly StringBuilder _buffer = new StringBuilder(32);
        private readonly List<Selectors> _selectors = new List<Selectors>(50);

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public CssParser(string css) {
            _css = css;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Parse the CSS text that was passed into the constructor and return the results.
        /// </summary>
        public List<Selectors> ParseCss() {

            Selectors currSelectors = null;
            Property currProperty = null;

            for (_index = 0; _index < _css.Length; _index++)
            {
                SkipWhiteSpace();
                var ch = Char();

                // check for comments
                if (ch == '/')
                {
                    if (Char(1) != '*')
                        throw new CssParseException("Illegal character detected in CSS.");

                    SkipComment();
                }
                // parse selectors
                else if (_mode == ParseMode.None)
                {
                    if (ch == '}')
                        break;

                    currSelectors = ParseSelectors();
                    if (currSelectors == null)
                    {
                        continue;
                    }

                    _mode = ParseMode.ParseProperty;
                }
                // parse property name
                else if (_mode == ParseMode.ParseProperty)
                {
                    currProperty = ParseProperty();

                    if (currProperty.IsLast)
                    {
                        _selectors.Add(currSelectors);
                        currSelectors = null;
                        currProperty = null;
                        _mode = ParseMode.None;
                        continue;
                    }

                    _mode = ParseMode.ParsePropertyValue;
                }
                // parse property value
                else if (_mode == ParseMode.ParsePropertyValue)
                {
                    var property = ParsePropertyValue(currProperty);

                    if (currProperty != null && currSelectors != null)
                    {
                        currSelectors.PropertyList.Add(currProperty);
                        currProperty = null;
                    }

                    if (property.IsLast)
                    {
                        _selectors.Add(currSelectors);
                        currSelectors = null;
                        _mode = ParseMode.None;
                    }
                    else
                    {
                        _mode = ParseMode.ParseProperty;
                    }
                }
            }

            return _selectors;
        }

        private void SkipWhiteSpace() {
            var hasReturn = false;
            for (; _index < _css.Length; _index++)
            {
                var ch = Char();

                if (" \t\n\r".IndexOf(ch) != -1)
                {
                    if (!hasReturn && " \t".IndexOf(ch) == -1)
                        hasReturn = true;
                    continue;
                }
                break;
            }
        }

        private void SkipComment() {
            for (; _index < _css.Length; _index++)
            {
                var ch = Char();

                if (ch == '/' && Char(-1) == '*') {
                    break;
                }
            }
        }

        private char Char(int offset = 0) {
            if (_index + offset < 0 || _index + offset >= _css.Length)
                return (char) 0;

            return _css[_index + offset];
        }

        private void TrimBuffer() {
            while (_buffer.Length > 0
                   && " \r\n\t".IndexOf(_buffer[_buffer.Length - 1]) != -1)
            {
                _buffer.Length = _buffer.Length - 1;
            }
        }

        private Selectors ParseSelectors() {
            _buffer.Length = 0;
            var result = new Selectors();
            var selectors = result.SelectorList;
            var isStarted = false;

            for (; _index < _css.Length; _index++)
            {
                var ch = Char();
                if (ch == '{')
                {
                    TrimBuffer();
                    if (_buffer.Length > 0)
                        selectors.Add(new CssSelector(_buffer.ToString()));
                    return result;
                }

                if (ch == ',')
                {
                    TrimBuffer();
                    selectors.Add(new CssSelector(_buffer.ToString()));
                    _buffer.Length = 0;
                    isStarted = false;
                    continue;
                }

                if (ch == '}')
                    break;

                // don't add leading whitespace
                if (!isStarted && " \r\n\t".IndexOf(ch) != -1)
                    continue;

                isStarted = true;
                _buffer.Append(ch);
            }

            TrimBuffer();
            if (_buffer.Length != 0)
            {
                throw new CssParseException("End of document reached prematurely while parsing css.");
            }

            return null;
        }

        private Property ParseProperty() {

            var property = new Property();
            _buffer.Length = 0;

            for (; _index < _css.Length; _index++)
            {
                var ch = Char();

                if (ch == ':')
                {
                    TrimBuffer();
                    property.Name = _buffer.ToString();
                    return property;
                }

                if (ch == '}')
                {
                    TrimBuffer();
                    if (_buffer.Length != 0)
                    {
                        throw new CssParseException("Premature end while parsing property: " + _buffer);
                    }

                    property.Name = _buffer.ToString();
                    property.IsLast = true;
                    return property;
                }

                _buffer.Append(ch);
            }

            throw new CssParseException("End of document reached prematurely while parsing property: " + _buffer);
        }

        private Property ParsePropertyValue(Property property)
        {
            _buffer.Length = 0;
            var mode = ValueParseMode.Value;
            var quote = (char) 0;
            var ch = (char) 0;

            for (; _index < _css.Length; _index++)
            {
                ch = Char();

                if (ch == '\'' || ch == '"')
                {
                    if (mode == ValueParseMode.Literal && quote == ch)
                    {
                        mode = ValueParseMode.Value;
                        continue;
                    }

                    if (mode == ValueParseMode.Value)
                    {
                        mode = ValueParseMode.Literal;
                        quote = ch;
                        continue;
                    }
                }
                else if ((ch == ';' || ch == '}') && mode == ValueParseMode.Value)
                {
                    break;
                }

                _buffer.Append(ch);
            }

            property.Value = _buffer.ToString();
            property.IsLast = property.IsLast || ch == '}';
            return property;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the parse results.
        /// </summary>
        public List<Selectors> ParseResult
        {
            get { return _selectors; }
        }

        #endregion

        #region Enums

        private enum ParseMode
        {
            None,
            ParseProperty,
            ParsePropertyValue
        }

        private enum ValueParseMode
        {
            Value,
            Literal
        }

        #endregion

        #region Classes

        public class Selectors
        {
            public readonly List<CssSelector> SelectorList = new List<CssSelector>();
            public readonly List<Property> PropertyList = new List<Property>();
        }

        public class Property
        {
            public string Name;
            public string Value;
            public bool IsLast;
        }

        #endregion
    }
}