using System;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using System.Text;
using Marklight;

namespace MarkLight.ValueConverters
{
    /// <summary>
    /// Value converter for Color type.
    /// </summary>
    public class ColorValueConverter : ValueConverter
    {
        #region Fields

        public static Dictionary<string, Color> ColorCodes;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ColorValueConverter()
        {
            _type = typeof(Color);
        }

        /// <summary>
        /// Initializes a static instance of the class.
        /// </summary>
        static ColorValueConverter()
        {
            ColorCodes = new Dictionary<string, Color>
            {
                {"clear", Color.clear},
                {"grey", Color.grey},
                {"red", Color.red},
                {"transparent", Color.clear},
                {"aliceblue", new Color(0.9411765f, 0.972549f, 1f)},
                {"antiquewhite", new Color(0.9803922f, 0.9215686f, 0.8431373f)},
                {"aquamarine", new Color(0.4980392f, 1f, 0.8313726f)},
                {"azure", new Color(0.9411765f, 1f, 1f)},
                {"beige", new Color(0.9607843f, 0.9607843f, 0.8627451f)},
                {"black", new Color(0f, 0f, 0f)},
                {"blue", new Color(0f, 0f, 1f)},
                {"blueviolet", new Color(0.5411765f, 0.1686275f, 0.8862745f)},
                {"brown", new Color(0.6470588f, 0.1647059f, 0.1647059f)},
                {"burlywood", new Color(0.8705882f, 0.7215686f, 0.5294118f)},
                {"chartreuse", new Color(0.4980392f, 1f, 0f)},
                {"coral", new Color(1f, 0.4980392f, 0.3137255f)},
                {"cornflowerblue", new Color(0.3921569f, 0.5843138f, 0.9294118f)},
                {"cyan", new Color(0f, 1f, 1f)},
                {"denim", new Color(0.08235294f, 0.3764706f, 0.7411765f)},
                {"dimgray", new Color(0.4117647f, 0.4117647f, 0.4117647f)},
                {"dodgerblue", new Color(0.1176471f, 0.5647059f, 1f)},
                {"electricindigo", new Color(0.4352941f, 0f, 1f)},
                {"firebrick", new Color(0.6980392f, 0.1333333f, 0.1333333f)},
                {"floralwhite", new Color(1f, 0.9803922f, 0.9411765f)},
                {"forestgreen", new Color(0.1333333f, 0.5450981f, 0.1333333f)},
                {"gainsboro", new Color(0.8627451f, 0.8627451f, 0.8627451f)},
                {"ghostwhite", new Color(0.972549f, 0.972549f, 1f)},
                {"gold", new Color(1f, 0.8431373f, 0f)},
                {"goldenrod", new Color(0.854902f, 0.6470588f, 0.1254902f)},
                {"gray", new Color(0.7450981f, 0.7450981f, 0.7450981f)},
                {"gray10", new Color(0.1019608f, 0.1019608f, 0.1019608f)},
                {"gray15", new Color(0.1490196f, 0.1490196f, 0.1490196f)},
                {"gray20", new Color(0.2f, 0.2f, 0.2f)},
                {"gray25", new Color(0.2509804f, 0.2509804f, 0.2509804f)},
                {"gray30", new Color(0.3019608f, 0.3019608f, 0.3019608f)},
                {"gray35", new Color(0.3490196f, 0.3490196f, 0.3490196f)},
                {"gray40", new Color(0.4f, 0.4f, 0.4f)},
                {"gray45", new Color(0.4509804f, 0.4509804f, 0.4509804f)},
                {"gray50", new Color(0.4980392f, 0.4980392f, 0.4980392f)},
                {"gray55", new Color(0.5490196f, 0.5490196f, 0.5490196f)},
                {"gray60", new Color(0.6f, 0.6f, 0.6f)},
                {"gray65", new Color(0.6509804f, 0.6509804f, 0.6509804f)},
                {"gray70", new Color(0.7019608f, 0.7019608f, 0.7019608f)},
                {"gray75", new Color(0.7490196f, 0.7490196f, 0.7490196f)},
                {"gray80", new Color(0.8f, 0.8f, 0.8f)},
                {"gray85", new Color(0.8509804f, 0.8509804f, 0.8509804f)},
                {"gray90", new Color(0.8980392f, 0.8980392f, 0.8980392f)},
                {"gray95", new Color(0.9490196f, 0.9490196f, 0.9490196f)},
                {"green", new Color(0f, 1f, 0f)},
                {"greenyellow", new Color(0.6784314f, 1f, 0.1843137f)},
                {"honeydew", new Color(0.9411765f, 1f, 0.9411765f)},
                {"hotpink", new Color(1f, 0.4117647f, 0.7058824f)},
                {"indianred", new Color(0.8039216f, 0.3607843f, 0.3607843f)},
                {"indigo", new Color(0.2941177f, 0f, 0.509804f)},
                {"ivory", new Color(1f, 1f, 0.9411765f)},
                {"lavender", new Color(0.9019608f, 0.9019608f, 0.9803922f)},
                {"lavenderblush", new Color(1f, 0.9411765f, 0.9607843f)},
                {"lawngreen", new Color(0.4862745f, 0.9882353f, 0f)},
                {"lemonchiffon", new Color(1f, 0.9803922f, 0.8039216f)},
                {"lightblue", new Color(0.6784314f, 0.8470588f, 0.9019608f)},
                {"lightcoral", new Color(0.9411765f, 0.5019608f, 0.5019608f)},
                {"lightpink", new Color(1f, 0.7137255f, 0.7568628f)},
                {"lightsalmon", new Color(1f, 0.627451f, 0.4784314f)},
                {"lightseagreen", new Color(0.1254902f, 0.6980392f, 0.6666667f)},
                {"lightskyblue", new Color(0.5294118f, 0.8078431f, 0.9803922f)},
                {"lightslateblue", new Color(0.5176471f, 0.4392157f, 1f)},
                {"lightslategray", new Color(0.4666667f, 0.5333334f, 0.6f)},
                {"lightsteelblue", new Color(0.6901961f, 0.7686275f, 0.8705882f)},
                {"lightyellow", new Color(1f, 1f, 0.8784314f)},
                {"limegreen", new Color(0.1960784f, 0.8039216f, 0.1960784f)},
                {"linen", new Color(0.9803922f, 0.9411765f, 0.9019608f)},
                {"magenta", new Color(1f, 0f, 1f)},
                {"maroon", new Color(0.6901961f, 0.1882353f, 0.3764706f)},
                {"midnightblue", new Color(0.09803922f, 0.09803922f, 0.4392157f)},
                {"mintcream", new Color(0.9607843f, 1f, 0.9803922f)},
                {"mistyrose", new Color(1f, 0.8941177f, 0.8823529f)},
                {"moccasin", new Color(1f, 0.8941177f, 0.7098039f)},
                {"navyblue", new Color(0f, 0f, 0.5019608f)},
                {"orange", new Color(1f, 0.6470588f, 0f)},
                {"orangered", new Color(1f, 0.2705882f, 0f)},
                {"orchid", new Color(0.854902f, 0.4392157f, 0.8392157f)},
                {"papayawhip", new Color(1f, 0.9372549f, 0.8352941f)},
                {"peachpuff", new Color(1f, 0.854902f, 0.7254902f)},
                {"pink", new Color(1f, 0.7529412f, 0.7960784f)},
                {"plum", new Color(0.8666667f, 0.627451f, 0.8666667f)},
                {"powderblue", new Color(0.6901961f, 0.8784314f, 0.9019608f)},
                {"purple", new Color(0.627451f, 0.1254902f, 0.9411765f)},
                {"rebeccapurple", new Color(0.4f, 0.2f, 0.6f)},
                {"rosybrown", new Color(0.7372549f, 0.5607843f, 0.5607843f)},
                {"royalblue", new Color(0.254902f, 0.4117647f, 0.8823529f)},
                {"saddlebrown", new Color(0.5450981f, 0.2705882f, 0.07450981f)},
                {"salmon", new Color(0.9803922f, 0.5019608f, 0.4470588f)},
                {"sandybrown", new Color(0.9568627f, 0.6431373f, 0.3764706f)},
                {"seagreen", new Color(0.3294118f, 1f, 0.6235294f)},
                {"seashell", new Color(1f, 0.9607843f, 0.9333333f)},
                {"silver", new Color(0.7529412f, 0.7529412f, 0.7529412f)},
                {"sienna", new Color(0.627451f, 0.3215686f, 0.1764706f)},
                {"skyblue", new Color(0.5294118f, 0.8078431f, 0.9215686f)},
                {"slategray", new Color(0.4392157f, 0.5019608f, 0.5647059f)},
                {"snow", new Color(1f, 0.9803922f, 0.9803922f)},
                {"springgreen", new Color(0f, 1f, 0.4980392f)},
                {"steelblue", new Color(0.2745098f, 0.509804f, 0.7058824f)},
                {"thistle", new Color(0.8470588f, 0.7490196f, 0.8470588f)},
                {"tropicalindigo", new Color(0.5882353f, 0.5137255f, 0.9254902f)},
                {"turquoise", new Color(0.2509804f, 0.8784314f, 0.8156863f)},
                {"violet", new Color(0.9333333f, 0.509804f, 0.9333333f)},
                {"violetred", new Color(0.8156863f, 0.1254902f, 0.5647059f)},
                {"wheat", new Color(0.9607843f, 0.8705882f, 0.7019608f)},
                {"white", new Color(1f, 1f, 1f)},
                {"whitesmoke", new Color(0.9607843f, 0.9607843f, 0.9607843f)},
                {"yellow", new Color(1f, 1f, 0f)},
                {"yellowgreen", new Color(0.6039216f, 0.8039216f, 0.1960784f)}
            };
        }

        #endregion

        #region Methods

        /// <summary>
        /// Value converter for Vector2 type.
        /// </summary>
        public override ConversionResult Convert(object value, ValueConverterContext context)
        {
            if (value == null)
                return base.Convert(null, context);

            var valueType = value.GetType();
            if (valueType == _type)
                return base.Convert(value, context);

            if (valueType != _stringType)
                return ConversionFailed(value);

            var stringValue = value as string;
            if (String.IsNullOrEmpty(stringValue))
                return ConversionFailed(value);

            stringValue = stringValue.Trim();

            try
            {
                Color color;

                // supported formats: #aarrggbb | #rrggbbaa | #rrggbb | ColorName | rgba(r,g,b,a) | r,g,b,a | r g b a

                // color hex
                if (stringValue[0] == '#')
                {
                    return TryParseHexColor(stringValue, context.HexColorType, out color)
                        ? new ConversionResult(color)
                        : StringConversionFailed(stringValue);
                }

                // color name
                if (ColorCodes.TryGetValue(stringValue.ToLower(), out color))
                    return new ConversionResult(color);

                if (stringValue[0] == 'r')
                {
                    // CSS rgba color function
                    if (TryParseColorFunction(stringValue, out color, context.ParseBuffer))
                        return new ConversionResult(color);
                }

                // comma or space delimited component values
                if (TryParseColorComponents(stringValue, out color, context.ParseBuffer))
                    return new ConversionResult(color);

                return StringConversionFailed(value);

            }
            catch (Exception e)
            {
                return ConversionFailed(value, e);
            }
        }

        /// <summary>
        /// Converts value to string.
        /// </summary>
        public override string ConvertToString(object value)
        {
            var color = (Color)value;
            return String.Format("{0},{1},{2},{3}", color.r, color.g, color.b, color.a);
        }

        /// <summary>
        /// Parse color from hex value. i.e "#FFFF55FF"
        /// </summary>
        public static bool TryParseHexColor(string hex, HexColorType type, out Color color)
        {
            color = new Color();

            if (hex == null)
                return false;

            hex = hex.Trim();

            if (String.IsNullOrEmpty(hex))
                return false;

            if (hex[0] != '#')
                return false;

            if (hex.Length == 9)
            {

                if (type == HexColorType.ARGB)
                {
                    // #aarrggbb
                    color = new Color(
                        int.Parse(hex.Substring(3, 2), NumberStyles.HexNumber) / 255f,
                        int.Parse(hex.Substring(5, 2), NumberStyles.HexNumber) / 255f,
                        int.Parse(hex.Substring(7, 2), NumberStyles.HexNumber) / 255f,
                        int.Parse(hex.Substring(1, 2), NumberStyles.HexNumber) / 255f
                    );
                }
                else
                {
                    // #rrggbbaa
                    color = new Color(
                        int.Parse(hex.Substring(1, 2), NumberStyles.HexNumber) / 255f,
                        int.Parse(hex.Substring(3, 2), NumberStyles.HexNumber) / 255f,
                        int.Parse(hex.Substring(5, 2), NumberStyles.HexNumber) / 255f,
                        int.Parse(hex.Substring(7, 2), NumberStyles.HexNumber) / 255f
                    );
                }
                return true;
            }

            if (hex.Length == 7)
            {
                // #rrggbb
                color = new Color(
                    int.Parse(hex.Substring(1, 2), NumberStyles.HexNumber) / 255f,
                    int.Parse(hex.Substring(3, 2), NumberStyles.HexNumber) / 255f,
                    int.Parse(hex.Substring(5, 2), NumberStyles.HexNumber) / 255f
                );
                return true;
            }

            return false;
        }

        /// <summary>
        /// Try parsing a string for a color function. ie. "rgba(255, 255, 255, 0.5)"
        /// Color components are an 8 bit number (0 - 255) and alpha component is a single (0-1).
        /// </summary>
        public static bool TryParseColorFunction(string colorStr, out Color color, StringBuilder buffer = null)
        {
            color = new Color();

            if (colorStr == null)
                return false;

            colorStr = colorStr.Trim();

            if (colorStr.Length < 13)
                return false;

            if (!colorStr.StartsWith("rgba(") || !colorStr.EndsWith(")"))
                return false;

            var values = new string[4];

            if (ParseUtils.ParseDelimitedValues(colorStr, values, 5, colorStr.Length - 2, buffer) != 4)
                return false;

            byte r;
            byte g;
            byte b;
            float a;

            if (!byte.TryParse(values[0], out r))
                return false;

            if (!byte.TryParse(values[1], out g))
                return false;

            if (!byte.TryParse(values[2], out b))
                return false;

            if (!float.TryParse(values[3], out a))
                return false;

            color = new Color(r / 255f, g /255f, b / 255f, a);

            return true;
        }

        /// <summary>
        /// Try parsing a string for space or comma delimited color components. ie. "1, 1, 0.75, 0.5"
        /// All components are values between 0 and 1 inclusive.
        /// </summary>
        public static bool TryParseColorComponents(string colorStr, out Color color, StringBuilder buffer = null)
        {
            color = new Color();

            if (colorStr == null)
                return false;

            var values = new string[4];

            var size = ParseUtils.ParseDelimitedValues(colorStr, values, -1, -1, buffer);
            if (size != 3 && size != 4)
                return false;

            float r;
            float g;
            float b;
            var a = 1f;

            if (!float.TryParse(values[0], out r))
                return false;

            if (!float.TryParse(values[1], out g))
                return false;

            if (!float.TryParse(values[2], out b))
                return false;

            if (size == 4)
            {
                if (!float.TryParse(values[3], out a))
                    return false;
            }

            color = new Color(r, g, b, a);

            return true;
        }

        #endregion
    }
}
