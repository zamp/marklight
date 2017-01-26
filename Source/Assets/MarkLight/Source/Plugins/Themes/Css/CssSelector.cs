using System.Collections.Generic;
using System.Text;

namespace Marklight.Themes
{
    /// <summary>
    /// Parses a combinated selector string into seperate selectors and keeps information about the
    /// type of combinator used.
    /// </summary>
    public class CssSelector
    {
        #region Private Fields

        private readonly string _raw;
        private readonly List<string> _selectors = new List<string>(5);
        private readonly List<StyleCombinatorType> _combinators = new List<StyleCombinatorType>(5);

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="selector">The combinated or non-combinated selector string.</param>
        public CssSelector(string selector) {

            _raw = selector;

            var buffer = new StringBuilder(_raw.Length);
            var combinator = StyleCombinatorType.None;
            var nextCombinator = StyleCombinatorType.None;
            var hasCombinator = false;

            for (var i = 0; i < _raw.Length; i++)
            {
                var ch = _raw[i];

                if (ch == ' ')
                {
                    if (nextCombinator < StyleCombinatorType.Descendant)
                        nextCombinator = StyleCombinatorType.Descendant;

                    hasCombinator = true;
                    continue;
                }

                if (ch == '>')
                {
                    if (nextCombinator < StyleCombinatorType.Child)
                        nextCombinator = StyleCombinatorType.Child;

                    hasCombinator = true;
                    continue;
                }

                if (hasCombinator)
                {
                    hasCombinator = false;
                    _selectors.Add(buffer.ToString());
                    _combinators.Add(combinator);
                    buffer.Length = 0;
                    combinator = nextCombinator;
                    nextCombinator = StyleCombinatorType.None;
                }

                buffer.Append(ch);
            }

            if (buffer.Length <= 0)
                return;

            _selectors.Add(buffer.ToString());
            _combinators.Add(combinator);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get the raw combinator selector string that was passed into the constructor.
        /// </summary>
        public string Raw
        {
            get { return _raw; }
        }

        /// <summary>
        /// Get a list of selectors in the order of combination.
        /// </summary>
        public List<string> Selectors
        {
            get { return _selectors; }
        }

        /// <summary>
        /// Get a list of combinator types whose index position corresponds to the index position
        /// of the selectors in the list returned by the Selectors property.
        /// </summary>
        public List<StyleCombinatorType> Combinators
        {
            get { return _combinators; }
        }

        #endregion
    }
}