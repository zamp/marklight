using System;
using UnityEngine;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MarkLight.ValueConverters
{
    /// <summary>
    /// Value converter for Font type.
    /// </summary>
    public class AssetValueConverter : ValueConverter
    {
        #region Fields

        protected Type _loadType;
        protected Type _unityAssetType;

        public bool IsUnityAssetType;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public AssetValueConverter()
        {
            _type = typeof(UnityAsset);
            _unityAssetType = typeof(UnityAsset);
            _loadType = _unityAssetType;
            IsUnityAssetType = true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Value converter for Font type.
        /// </summary>
        public override ConversionResult Convert(object value, ValueConverterContext context)
        {
            if (value == null)
                return base.Convert(null, context);

            var valueType = value.GetType();
            if (valueType == _type)
                return base.Convert(value, context);

            var stringValue = value as string;

            if (stringValue != null)
            {
                return ConvertFromString(stringValue, context);
            }

            if (valueType == _loadType)
            {
                // is asset pre-loaded? 
                var unityAsset = ViewPresenter.Instance.GetAsset(value as UnityEngine.Object);
                return ConvertAssetResult(unityAsset ?? new UnityAsset(String.Empty, value as UnityEngine.Object));
            }

            return ConvertCustomType(value, valueType, context);
        }

        /// <summary>
        /// Used to extend the asset value converter with custom types.
        /// </summary>
        protected virtual ConversionResult ConvertCustomType(object value, Type valueType, ValueConverterContext context)
        {
            return ConversionFailed(value);
        }

        /// <summary>
        /// Converts asset path and sets bool indicating if asset not found errors should be suppressed.
        /// </summary>
        protected virtual string ConvertAssetPath(string loadAssetPath, bool inResourcesFolder,
                                                    out bool suppressAssetNotFoundError)
        {
            suppressAssetNotFoundError = false;
            return loadAssetPath;
        }

        /// <summary>
        /// Converts loaded asset to desired type.
        /// </summary>
        protected virtual ConversionResult ConvertAssetResult(UnityAsset loadedAsset)
        {
            return IsUnityAssetType
                ? new ConversionResult(loadedAsset)
                : new ConversionResult(loadedAsset.Asset);
        }

        /// <summary>
        /// Converts value to string.
        /// </summary>
        public override string ConvertToString(object value)
        {
            if (!IsUnityAssetType)
                return ViewPresenter.Instance.GetAssetPath(value as UnityEngine.Object);

            var asset = value as UnityAsset;
            return asset != null ? asset.ToString() : String.Empty;
        }

        private ConversionResult ConvertFromString(string stringValue, ValueConverterContext context)
        {
            try
            {
                var assetPath = stringValue.Trim();
                if (String.IsNullOrEmpty(assetPath))
                    return new ConversionResult(null);

                var isOnDemandLoaded = assetPath.StartsWith("?");
                if (isOnDemandLoaded)
                {
                    assetPath = assetPath.Substring(1);
                }
                else if (!String.IsNullOrEmpty(context.BaseDirectory))
                {
                    assetPath = Path.Combine(context.BaseDirectory, assetPath);
                }

                // is asset pre-loaded?
                var unityAsset = ViewPresenter.Instance.GetAsset(assetPath);
                if (unityAsset != null)
                {
                    // yes. return pre-loaded asset
                    return ConvertAssetResult(unityAsset);
                }

                // is asset to be loaded externally on-demand?
                if (isOnDemandLoaded)
                {
                    // yes.
                    unityAsset = ViewPresenter.Instance.AddAsset(assetPath, null);
                    return ConvertAssetResult(unityAsset);
                }

                // if the asset is in a resources folder the load path should be relative to the folder
                var inResourcesFolder = assetPath.Contains("Resources/");
                var loadAssetPath = assetPath;
                if (inResourcesFolder)
                {
                    loadAssetPath = loadAssetPath.Substring(
                        assetPath.IndexOf("Resources/", StringComparison.Ordinal) + 10);

                    var extension = Path.GetExtension(assetPath);
                    if (extension.Length > 0)
                    {
                        loadAssetPath = loadAssetPath.Substring(0, loadAssetPath.Length - extension.Length);
                    }
                }

                bool suppressAssetNotFoundError;
                loadAssetPath = ConvertAssetPath(loadAssetPath, inResourcesFolder, out suppressAssetNotFoundError);

                // load asset from asset database
                if (Application.isPlaying && !inResourcesFolder)
                {
                    return suppressAssetNotFoundError
                        ? new ConversionResult(null)
                        : ConversionFailed(stringValue,
                            String.Format("Pre-loaded asset not found for path \"{0}\".", assetPath));
                }

                UnityEngine.Object asset;

                // load font from asset database
#if UNITY_EDITOR
                asset = inResourcesFolder ? Resources.Load(loadAssetPath, _loadType) : AssetDatabase.LoadAssetAtPath(loadAssetPath, _loadType);
#else
                asset = Resources.Load(loadAssetPath);
#endif
                if (asset == null)
                {
                    return suppressAssetNotFoundError
                        ? new ConversionResult(null)
                        : ConversionFailed(stringValue, String.Format("Asset not found at path \"{0}\".", assetPath));
                }

                var loadedAsset = ViewPresenter.Instance.AddAsset(assetPath, asset);
                return ConvertAssetResult(loadedAsset);
            }
            catch (Exception e)
            {
                return ConversionFailed(stringValue, e);
            }
        }

        #endregion
    }
}
