using MarkLight.ValueConverters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace MarkLight.Views.UI
{
    /// <summary>
    /// Label view.
    /// </summary>
    /// <d>Presents (read-only) text. Can adjust its size to text. Can display rich text with BBCode style syntax.</d>
    [ExcludeComponent("ImageComponent")]
    [HideInPresenter]
    public class Label : UIView
    {
        #region Fields

        #region TextComponent

        /// <summary>
        /// Label text.
        /// </summary>
        /// <d>The text of the label. The label can be set to adjust its size to the text through the AdjustToText field.</d>
        [MapTo("TextComponent.text", "TextChanged")]
        public _string Text;

        /// <summary>
        /// Label text font.
        /// </summary>
        /// <d>The font of the label text.</d>
        [MapTo("TextComponent.font", "TextStyleChanged")]
        public _Font Font;

        /// <summary>
        /// Label text font size.
        /// </summary>
        /// <d>The size of the label text.</d>
        [MapTo("TextComponent.fontSize", "TextStyleChanged")]
        public _int FontSize;

        /// <summary>
        /// Label text line spacing.
        /// </summary>
        /// <d>The line spacing of the label text.</d>
        [MapTo("TextComponent.lineSpacing", "TextStyleChanged")]
        public _int LineSpacing;

        /// <summary>
        /// Support rich text.
        /// </summary>
        /// <d>Boolean indicating if the label supports rich text.</d>
        [MapTo("TextComponent.supportRichText")]
        public _bool SupportRichText;

        /// <summary>
        /// Label text font color.
        /// </summary>
        /// <d>The font color of the label text.</d>
        [MapTo("TextComponent.color")]
        public _Color FontColor;

        /// <summary>
        /// Label text font style.
        /// </summary>
        /// <d>The font style of the label text.</d>
        [MapTo("TextComponent.fontStyle", "TextStyleChanged")]
        public _FontStyle FontStyle;

        /// <summary>
        /// Align by glyph geometry.
        /// </summary>
        /// <d>Boolean indicating if the extents of glyph geometry should be used to perform horizontal alignment rather than glyph metrics.</d>
        [MapTo("TextComponent.alignByGeometry")]
        public _bool AlignByGeometry;

        /// <summary>
        /// Resize text for best fit.
        /// </summary>
        /// <d>Boolean indicating if the text is to be automatically resized to fill the size of the label.</d>
        [MapTo("TextComponent.resizeTextForBestFit")]
        public _bool ResizeTextForBestFit;

        /// <summary>
        /// Resize text max size.
        /// </summary>
        /// <d>If ResizeTextForBestFit is true this indicates the maximum size the text can be, 1 = infinity.</d>
        [MapTo("TextComponent.resizeTextMaxSize")]
        public _int ResizeTextMaxSize;

        /// <summary>
        /// Resize text min size.
        /// </summary>
        /// <d>If ResizeTextForBestFit is true this indicates the minimum size the text can be.<d>
        [MapTo("TextComponent.resizeTextMinSize")]
        public _int ResizeTextMinSize;

#if !UNITY_4_6
        /// <summary>
        /// Horizontal overflow mode.
        /// </summary>
        /// <d>Indicates what will happen if the text overflows the horizontal bounds of the label.<d>
        [MapTo("TextComponent.horizontalOverflow")]
        public _HorizontalWrapMode HorizontalOverflow;

        /// <summary>
        /// Vertical overflow mode.
        /// </summary>
        /// <d>Indicates what will happen if the text overflows the vertical bounds of the label.<d>
        [MapTo("TextComponent.verticalOverflow")]
        public _VerticalWrapMode VerticalOverflow;
#endif

        /// <summary>
        /// Unity UI component used to render text.
        /// </summary>
        /// <d>Unity UI component used to render text.</d>
        public Text TextComponent;

        #endregion

        /// <summary>
        /// Label text alignment.
        /// </summary>
        /// <d>The alignment of the text inside the label. Can be used with TextMargin and TextOffset to get desired positioning of the text.</d>
        [ChangeHandler("BehaviorChanged")]
        public _ElementAlignment TextAlignment;

        /// <summary>
        /// Label text shadow color.
        /// </summary>
        /// <d>The shadow color of the label text.</d>
        [ChangeHandler("BehaviorChanged")]
        public _Color ShadowColor;

        /// <summary>
        /// Label text shadow distance.
        /// </summary>
        /// <d>The distance of the label text shadow.</d>
        [ChangeHandler("BehaviorChanged")]
        public _Vector2 ShadowDistance;

        /// <summary>
        /// Label text outline color.
        /// </summary>
        /// <d>The outline color of the label text.</d>
        [ChangeHandler("BehaviorChanged")]
        public _Color OutlineColor;

        /// <summary>
        /// Label text outline distance.
        /// </summary>
        /// <d>The distance of the label text outline.</d>
        [ChangeHandler("BehaviorChanged")]
        public _Vector2 OutlineDistance;

        /// <summary>
        /// Adjust label to text.
        /// </summary>
        /// <d>An enum indiciating how the label should adjust its size to the label text. By default the label does not adjust its size to the text.</d>
        [ChangeHandler("LayoutChanged")]
        public _AdjustToText AdjustToText;

        private static readonly Regex _tagRegex = new Regex(@"\[(?<tag>[^\]]+)\]");
        private static readonly ListPool<TextToken> _tokenBuffers = new ListPool<TextToken>();
        private static readonly StackPool<int> _fontSizeBuffers = new StackPool<int>();
        private static readonly string[] _seperatorString = {"&sp;"};

        #endregion

        #region Methods

        /// <summary>
        /// Sets default values of the view.
        /// </summary>
        public override void SetDefaultValues()
        {
            base.SetDefaultValues();

            Width.DirectValue = ElementSize.FromPixels(120);
            Height.DirectValue = ElementSize.FromPixels(40);

            TextAlignment.DirectValue = ElementAlignment.Left;
            if (TextComponent != null)
            {
                TextComponent.fontSize = 18;
                TextComponent.lineSpacing = 1;
                TextComponent.color = Color.black;

#if !UNITY_4_6_0 && !UNITY_4_6_1 && !UNITY_4_6_2
                TextComponent.horizontalOverflow = HorizontalWrapMode.Overflow;
                TextComponent.verticalOverflow = VerticalWrapMode.Overflow;
#endif
            }
        }

        /// <summary>
        /// Called when text changes.
        /// </summary>
        public virtual void TextChanged()
        {
            // parse text
            Text.DirectValue = ParseText(Text.Value);
            if (AdjustToText == MarkLight.AdjustToText.None)
            {
                // size of view doesn't change with text, no need to notify parents
                QueueChangeHandler("LayoutChanged");
            }
            else
            {
                // size of view changes with text so notify parents
                LayoutChanged();
            }
        }

        public override bool CalculateLayoutChanges(LayoutChangeContext context) {
            AdjustLabelToText();
            return base.CalculateLayoutChanges(context);
        }

        /// <summary>
        /// Called when the text style has changed.
        /// </summary>
        public virtual void TextStyleChanged()
        {
            if (AdjustToText == MarkLight.AdjustToText.None)
                return;

            LayoutChanged();
        }

        /// <summary>
        /// Called when a field affecting the behavior and visual appearance of the view has changed.
        /// </summary>
        public override void BehaviorChanged()
        {
            base.BehaviorChanged();

            if (TextComponent == null)
                return;

            TextComponent.alignment = TextAnchor;
            if (ShadowColor.IsSet || ShadowDistance.IsSet)
            {
                var shadowComponent = GetComponent<Shadow>();
                if (shadowComponent == null)
                {
                    shadowComponent = gameObject.AddComponent<Shadow>();
                }

                shadowComponent.effectColor = ShadowColor.Value;
                shadowComponent.effectDistance = ShadowDistance.Value;
            }

            if (OutlineColor.IsSet || OutlineDistance.IsSet)
            {
                var outlineComponent = GetComponent<Outline>();
                if (outlineComponent == null)
                {
                    outlineComponent = gameObject.AddComponent<Outline>();
                }

                outlineComponent.effectColor = OutlineColor.Value;
                outlineComponent.effectDistance = OutlineDistance.Value;
            }
        }

        /// <summary>
        /// Adjusts the label to text.
        /// </summary>
        public void AdjustLabelToText()
        {
            if (AdjustToText == MarkLight.AdjustToText.Width)
            {
                Layout.TargetWidth = ElementSize.FromPixels(PreferredWidth);
            }
            else if (AdjustToText == MarkLight.AdjustToText.Height)
            {
                Layout.TargetHeight = ElementSize.FromPixels(PreferredHeight);
            }
            else if (AdjustToText == MarkLight.AdjustToText.WidthAndHeight)
            {
                Layout.TargetWidth = ElementSize.FromPixels(PreferredWidth);
                Layout.TargetHeight = ElementSize.FromPixels(PreferredHeight);
            }
        }

        /// <summary>
        /// Replaces BBCode style tags with unity rich text syntax and parses embedded views.
        /// </summary>
        public string ParseText(string text)
        {
            if (text == null)
                return String.Empty;

            if (TextComponent == null || !TextComponent.supportRichText)
                return text;

            // search for tokens and apply formatting and embedded views
            var tokens = _tokenBuffers.Get();
            var formattedText = _tagRegex.Replace(text, x =>
            {
                var tag = x.Groups["tag"].Value.Trim();
                var tagNoWs = tag.RemoveWhitespace();

                // check if tag matches default tokens
                if (String.Equals("B", tagNoWs, StringComparison.OrdinalIgnoreCase))
                {
                    // bold
                    tokens.Add(new TextToken(TextTokenType.BoldStart));
                    return _seperatorString[0];
                }

                if (String.Equals("/B", tagNoWs, StringComparison.OrdinalIgnoreCase))
                {
                    // bold end
                    tokens.Add(new TextToken(TextTokenType.BoldEnd));
                    return _seperatorString[0];
                }

                if (String.Equals("I", tagNoWs, StringComparison.OrdinalIgnoreCase))
                {
                    // italic
                    tokens.Add(new TextToken(TextTokenType.ItalicStart));
                    return _seperatorString[0];
                }

                if (String.Equals("/I", tagNoWs, StringComparison.OrdinalIgnoreCase))
                {
                    // italic end
                    tokens.Add(new TextToken(TextTokenType.ItalicEnd));
                    return _seperatorString[0];
                }

                if (tagNoWs.StartsWith("SIZE=", StringComparison.OrdinalIgnoreCase))
                {
                    // parse size value
                    var vc = IntValueConverter.Instance;
                    var convertResult = vc.Convert(tagNoWs.Substring(5), ValueConverterContext.Default);
                    if (!convertResult.Success)
                    {
                        // unable to parse token
                        Debug.LogError(String.Format(
                                           "[MarkLight] {0}: Unable to parse text embedded size tag \"[{1}]\". {2}",
                                           GameObjectName, tag, convertResult.ErrorMessage));
                        return String.Format("[{0}]", tag);
                    }

                    tokens.Add(new TextToken(TextTokenType.SizeStart)
                    {
                        FontSize = (int) convertResult.ConvertedValue
                    });
                    return _seperatorString[0];
                }

                if (String.Equals("/SIZE", tagNoWs, StringComparison.OrdinalIgnoreCase))
                {
                    // size end
                    tokens.Add(new TextToken(TextTokenType.SizeEnd));
                    return _seperatorString[0];
                }

                if (tagNoWs.StartsWith("COLOR=", StringComparison.OrdinalIgnoreCase))
                {
                    // parse color value
                    var vc = ColorValueConverter.Instance;
                    var convertResult = vc.Convert(tagNoWs.Substring(6), ValueConverterContext.Default);
                    if (!convertResult.Success)
                    {
                        // unable to parse token
                        Debug.LogError(String.Format(
                                           "[MarkLight] {0}: Unable to parse text embedded color tag \"[{1}]\". {2}",
                                           GameObjectName, tag, convertResult.ErrorMessage));
                        return String.Format("[{0}]", tag);
                    }

                    var color = (Color)convertResult.ConvertedValue;
                    tokens.Add(new TextToken(TextTokenType.ColorStart) { FontColor = color });
                    return _seperatorString[0];
                }

                if (String.Equals("/COLOR", tagNoWs, StringComparison.OrdinalIgnoreCase))
                {
                    // color end
                    tokens.Add(new TextToken(TextTokenType.ColorEnd));
                    return _seperatorString[0];
                }

                return String.Format("[{0}]", tag);
            });

            // replace newline in string
            formattedText = formattedText.Replace("\\n", Environment.NewLine);

            // split the string up on each line
            var result = BufferPools.StringBuilders.Get();
            var splitString = formattedText.Split(_seperatorString, StringSplitOptions.None);
            var splitIndex = 0;
            var fontBoldCount = 0;
            var fontItalicCount = 0;
            var fontSizeStack = _fontSizeBuffers.Get();

            // loop through each split string and apply tokens (embedded views & font styles)
            foreach (var str in splitString)
            {
                var tokenIndex = splitIndex - 1;
                var token = tokenIndex >= 0 && tokenIndex < tokens.Count
                    ? tokens[tokenIndex]
                    : new TextToken();

                ++splitIndex;

                // do we have a token?
                if (!token.IsValid)
                {
                    // yes. parse token type
                    switch (token.Type)
                    {
                        case TextTokenType.BoldStart:
                            result.Append("<b>");
                            ++fontBoldCount;
                            break;

                        case TextTokenType.BoldEnd:
                            result.Append("</b>");
                            --fontBoldCount;
                            break;

                        case TextTokenType.ItalicStart:
                            result.Append("<i>");
                            ++fontItalicCount;
                            break;

                        case TextTokenType.ItalicEnd:
                            result.Append("</i>");
                            --fontItalicCount;
                            break;

                        case TextTokenType.SizeStart:
                            result.Append("<size=");
                            result.Append(token.FontSize);
                            result.Append('>');
                            fontSizeStack.Push(token.FontSize);
                            break;

                        case TextTokenType.SizeEnd:
                            result.Append("</size>");
                            fontSizeStack.Pop();
                            break;

                        case TextTokenType.ColorStart:
                            var r = (int) (token.FontColor.r * 255f);
                            var g = (int) (token.FontColor.g * 255f);
                            var b = (int) (token.FontColor.b * 255f);
                            var a = (int) (token.FontColor.a * 255f);
                            result.Append(String.Format("<color=#{0:X2}{1:X2}{2:X2}{3:X2}>", r, g, b, a));
                            break;

                        case TextTokenType.ColorEnd:
                            result.Append("</color>");
                            break;

                        case TextTokenType.EmbeddedView:
                            break;

                        case TextTokenType.Unknown:
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                result.Append(str);
            }


            var resultStr = result.ToString();

            _tokenBuffers.Recycle(tokens);
            BufferPools.StringBuilders.Recycle(result);
            _fontSizeBuffers.Recycle(fontSizeStack);

            return resultStr;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets text anchor.
        /// </summary>
        public TextAnchor TextAnchor
        {
            get
            {
                switch (TextAlignment.Value)
                {
                    case ElementAlignment.TopLeft:
                        return TextAnchor.UpperLeft;
                    case ElementAlignment.Top:
                        return TextAnchor.UpperCenter;
                    case ElementAlignment.TopRight:
                        return TextAnchor.UpperRight;
                    case ElementAlignment.Left:
                        return TextAnchor.MiddleLeft;
                    case ElementAlignment.Right:
                        return TextAnchor.MiddleRight;
                    case ElementAlignment.BottomLeft:
                        return TextAnchor.LowerLeft;
                    case ElementAlignment.Bottom:
                        return TextAnchor.LowerCenter;
                    case ElementAlignment.BottomRight:
                        return TextAnchor.LowerRight;
                    case ElementAlignment.Center:
                    default:
                        return TextAnchor.MiddleCenter;
                }
            }
        }

        /// <summary>
        /// Preferred width of text.
        /// </summary>
        public virtual float PreferredWidth
        {
            get
            {
                return TextComponent != null ? TextComponent.preferredWidth : 0;
            }
        }

        /// <summary>
        /// Preferred height of text.
        /// </summary>
        public virtual float PreferredHeight
        {
            get
            {
                return TextComponent != null ? TextComponent.preferredHeight : 0;
            }
        }

        #endregion
    }
}
