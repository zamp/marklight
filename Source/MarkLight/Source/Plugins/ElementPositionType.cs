namespace MarkLight
{
    /// <summary>
    /// Specifies how an element is positioned via its offsets.
    /// </summary>
    public enum ElementPositionType
    {
        /// <summary>
        /// The element offset positions the element relative to it's normal position, which may be affected by a
        /// parent container.
        /// </summary>
        Normal = 0,

        /// <summary>
        /// The element offset is an obsolute position within the parent container and is not affected by the parent
        /// child layout.
        /// </summary>
        Absolute = 1
    }
}