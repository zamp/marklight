namespace MarkLight
{
    /// <summary>
    /// Enum indicating type of list change action initiated.
    /// </summary>
    public enum ListChangeAction
    {
        /// <summary>
        /// Items added to list.
        /// </summary>
        Add = 0,

        /// <summary>
        /// Items moved (rearranged) within list.
        /// </summary>
        Move = 1,

        /// <summary>
        /// Items removed from list.
        /// </summary>
        Remove = 2,

        /// <summary>
        /// All items cleared from list.
        /// </summary>
        Clear = 3,

        /// <summary>
        /// Items modified in list.
        /// </summary>
        Modify = 4,

        /// <summary>
        /// Item selected in list.
        /// </summary>
        Select = 5,

        /// <summary>
        /// Item scrolled to.
        /// </summary>
        ScrollTo = 6
    }
}