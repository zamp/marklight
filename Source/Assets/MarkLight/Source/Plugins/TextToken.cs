using UnityEngine;

namespace MarkLight
{
    /// <summary>
    /// Contains information about a formatting token found in text.
    /// </summary>
    public struct TextToken
    {
        #region Fields

        public readonly TextTokenType Type;
        public readonly bool IsValid;

        public View EmbeddedView;
        public int FontSize;
        public Color FontColor;

        #endregion

        #region Constructor

        public TextToken(TextTokenType type) : this()
        {
            Type = type;
            IsValid = true;
        }

        #endregion
    }

    /// <summary>
    /// Text token type.
    /// </summary>
    public enum TextTokenType
    {
        /// <summary>
        /// Unknown text token.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Embedded view token.
        /// </summary>
        EmbeddedView = 1,

        /// <summary>
        /// Bold start token.
        /// </summary>
        BoldStart = 2,

        /// <summary>
        /// Bold end token.
        /// </summary>
        BoldEnd = 3,

        /// <summary>
        /// Italic start token.
        /// </summary>
        ItalicStart = 4,

        /// <summary>
        /// Italic end token.
        /// </summary>
        ItalicEnd = 5,

        /// <summary>
        /// Text size start token.
        /// </summary>
        SizeStart = 6,

        /// <summary>
        /// Text size end token.
        /// </summary>
        SizeEnd = 7,

        /// <summary>
        /// Text color start token.
        /// </summary>
        ColorStart = 8,

        /// <summary>
        /// Text color end token.
        /// </summary>
        ColorEnd = 9,
    }
}
