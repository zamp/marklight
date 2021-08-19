using UnityEngine;

namespace MarkLight.ValueConverters
{
    /// <summary>
    /// Value converter for Material type.
    /// </summary>
    public class MaterialValueConverter : AssetValueConverter
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public MaterialValueConverter()
        {
            _type = typeof(Material);
            _loadType = _type;
            IsUnityAssetType = false;
        }

        #endregion
    }
}
