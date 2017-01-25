using System;

namespace Marklight.Themes
{
    /// <summary>
    /// Exception thrown when there is a CSS parse error.
    /// </summary>
    public class CssParseException : Exception
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public CssParseException(string message) : base(message) {
        }
    }
}