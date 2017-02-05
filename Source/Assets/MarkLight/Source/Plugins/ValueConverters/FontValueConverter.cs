using UnityEngine;

namespace MarkLight.ValueConverters
{
    /// <summary>
    /// Value converter for Font type.
    /// </summary>
    public class FontValueConverter : AssetValueConverter
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public FontValueConverter()
        {
            _type = typeof(Font);
            _loadType = _type;
            IsUnityAssetType = false;
        }

        #endregion
    }
}
