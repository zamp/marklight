using System;
using UnityEngine;

namespace MarkLight.ValueConverters
{
    /// <summary>
    /// Value converter for Sprite type.
    /// </summary>
    public class SpriteValueConverter : AssetValueConverter
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public SpriteValueConverter()
        {
            _type = typeof(Sprite);
            _loadType = _type;
            IsUnityAssetType = false;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Used to extend the asset value converter with custom types.
        /// </summary>
        protected override ConversionResult ConvertCustomType(object value, Type valueType, ValueConverterContext context)
        {
            var spriteAsset = value as SpriteAsset;
            return spriteAsset != null ? new ConversionResult(spriteAsset.Sprite) : ConversionFailed(value);
        }

        #endregion
    }
}