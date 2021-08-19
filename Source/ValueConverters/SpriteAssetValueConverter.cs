using System;
using UnityEngine;

namespace MarkLight.ValueConverters
{
    /// <summary>
    /// Value converter for Sprite type.
    /// </summary>
    public class SpriteAssetValueConverter : AssetValueConverter
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public SpriteAssetValueConverter()
        {
            _type = typeof(SpriteAsset);
            _loadType = typeof(Sprite);
            IsUnityAssetType = true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Converts loaded asset to desired type.
        /// </summary>
        protected override ConversionResult ConvertAssetResult(UnityAsset loadedAsset)
        {
            return new ConversionResult(new SpriteAsset(loadedAsset));
        }

        /// <summary>
        /// Converts value to string.
        /// </summary>
        public override string ConvertToString(object value)
        {
            return value != null ? ((SpriteAsset)value).Path : String.Empty;
        }

        #endregion
    }
}