namespace Marklight.Themes
{
    /// <summary>
    /// Specifies what type of styles are applied by a theme.
    /// </summary>
    public enum StyleFilterMode
    {
        /// <summary>
        /// Apply all appropriate styles.
        /// </summary>
        All,
        /// <summary>
        /// Only apply styles that have a parent style.
        /// </summary>
        ChildOnly,
        /// <summary>
        /// Only apply styles that don't have a parent style.
        /// </summary>
        RootOnly
    }
}