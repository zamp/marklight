namespace Marklight.Themes
{
    /// <summary>
    /// Data object for a CSS file asset.
    /// </summary>
    public class CssAsset
    {
        /// <summary>
        /// The name of the file the css is parsed from.
        /// </summary>
        public readonly string Name;
        /// <summary>
        /// The contents of the css file.
        /// </summary>
        public readonly string Css;

        /// <summary>
        /// Cosntructor.
        /// </summary>
        public CssAsset(string name, string css) {
            Name = name;
            Css = css;
        }
    }
}