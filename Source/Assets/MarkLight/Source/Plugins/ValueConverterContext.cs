using System;
using System.Text;
using System.Xml.Linq;
using Marklight;
using MarkLight.ValueConverters;
using UnityEngine;

namespace MarkLight
{
    /// <summary>
    /// Contains information about the context in which a value conversion occurs.
    /// </summary>
    [Serializable]
    public class ValueConverterContext
    {
        #region Fields

        /// <summary>
        /// Default value converter context.
        /// </summary>
        public static ValueConverterContext Default = new ValueConverterContext(new StringBuilder(32));

        [SerializeField]
        private string _baseDirectory;

        [SerializeField]
        private Vector3 _unitSize;

        [SerializeField]
        private HexColorType _hexColorType;

        [SerializeField]
        private bool _hasUnitSize;

        [SerializeField]
        private bool _hasHexColorType;

        [NonSerialized]
        private StringBuilder _parseBuffer;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        protected ValueConverterContext() {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected ValueConverterContext(StringBuilder buffer) {
            _parseBuffer = buffer;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ValueConverterContext(ValueConverterContext parentContext,
                    string baseDirectory = null, Vector3? unitSize = null, HexColorType? hexColorType = null)
        {
            if (parentContext != null)
            {
                _baseDirectory = parentContext.BaseDirectory;
                _unitSize = parentContext.UnitSize;
                _hexColorType = parentContext.HexColorType;
                _parseBuffer = parentContext.ParseBuffer;
            }
            else
            {
                _parseBuffer = Default.ParseBuffer;
            }

            if (baseDirectory != null)
                _baseDirectory = baseDirectory;

            if (unitSize != null)
                _unitSize = unitSize.Value;

            if (hexColorType != null)
                _hexColorType = hexColorType.Value;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ValueConverterContext(string baseDirectory, Vector3 unitSize, HexColorType hexColorType)
        {
            _baseDirectory = baseDirectory;
            _unitSize = unitSize;
            _hexColorType = hexColorType;
            _parseBuffer = Default.ParseBuffer;
        }

        /// <summary>
        /// Creates value converter context from element settings.
        /// </summary>
        public ValueConverterContext(ValueConverterContext parentContext, XElement element, string viewName)
        {
            if (parentContext != null)
            {
                _baseDirectory = parentContext.BaseDirectory;
                _unitSize = parentContext.UnitSize;
                _hexColorType = parentContext.HexColorType;
                _parseBuffer = parentContext.ParseBuffer;
            }
            else
            {
                _parseBuffer = Default.ParseBuffer;
            }

            var baseDirectoryAttr = element.Attribute("BaseDirectory");
            var unitSizeAttr = element.Attribute("UnitSize");
            var colorHexAttr = element.Attribute("ColorHexType");

            if (baseDirectoryAttr != null)
                _baseDirectory = baseDirectoryAttr.Value;

            if (unitSizeAttr != null)
            {
                var unitSizeString = unitSizeAttr.Value;
                var converter = new Vector3ValueConverter();
                var result = converter.Convert(unitSizeString);
                if (result.Success)
                {
                    _unitSize = (Vector3) result.ConvertedValue;
                }
                else
                {
                    Debug.LogError(String.Format(
                        "[MarkLight] {0}: Error parsing XUML. Unable to parse UnitSize "+
                        "attribute value \"{1}\".",
                        viewName, unitSizeString));
                }
            }

            if (colorHexAttr != null)
            {
                var colorHexTypeString = colorHexAttr.Value;
                var result = EnumValueConverter.HexColorType.Convert(colorHexTypeString);
                if (result.Success)
                {
                    _unitSize = (Vector3) result.ConvertedValue;
                }
                else
                {
                    Debug.LogError(String.Format(
                        "[MarkLight] {0}: Error parsing XUML. Unable to parse ColorHexType "+
                        "attribute value \"{1}\".",
                        viewName, colorHexTypeString));
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// The base directory added as prefix to parsed file paths.
        /// </summary>
        public string BaseDirectory
        {
            get
            {
                return _baseDirectory ?? (_baseDirectory = ViewPresenter.Instance != null
                           ? ViewPresenter.Instance.BaseDirectory
                           : String.Empty);
            }
        }

        /// <summary>
        /// The unit size conversion.
        /// </summary>
        public Vector3 UnitSize
        {
            get
            {
                if (_hasUnitSize)
                    return _unitSize;

                _hasUnitSize = true;

                return _unitSize = ViewPresenter.Instance != null
                    ? ViewPresenter.Instance.UnitSize
                    : new Vector3(40f, 40f, 40f);
            }
        }

        /// <summary>
        /// The color hex component ordering type.
        /// </summary>
        public HexColorType HexColorType
        {
            get
            {
                if (_hasHexColorType)
                    return _hexColorType;

                _hasHexColorType = true;

                return _hexColorType = ViewPresenter.Instance != null
                    ? ViewPresenter.Instance.HexColorType
                    : HexColorType.ARGB;
            }
        }

        /// <summary>
        /// Shared parsing buffer for converters to use during conversion. Single thread only.
        /// </summary>
        public StringBuilder ParseBuffer
        {
            get { return _parseBuffer ?? (_parseBuffer = new StringBuilder(32)); }
        }

        /// <summary>
        /// Get the default base directory.
        /// </summary>
        public static string DefaultBaseDirectory
        {
            get { return String.Empty; }
        }

        /// <summary>
        /// Get the default unit size.
        /// </summary>
        public static Vector3 DefaultUnitSize
        {
            get { return new Vector3(40f, 40f, 40f); }
        }

        /// <summary>
        /// Get the default color hex type.
        /// </summary>
        public static HexColorType DefaultHexColorType
        {
            get { return HexColorType.ARGB; }
        }

        #endregion
    }
}
