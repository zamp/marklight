namespace Marklight.Themes
{
    /// <summary>
    /// Specifies the combinator used to combine selectors
    /// </summary>
    public enum StyleCombinatorType
    {
        /// <summary>
        /// No combinator.
        /// </summary>
        None = 0,
        /// <summary>
        /// Selector applies to any descendent of the previous selector.
        /// </summary>
        Descendant = 1,
        /// <summary>
        /// Selector only applies to direct children of the previous selector.
        /// </summary>
        Child = 2
    }
}