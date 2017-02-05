using System;
using System.Collections.Generic;
using MarkLight;
using MarkLight.ValueConverters;
using UnityEngine;

namespace Marklight.Themes
{
    public class CssThemeLoader
    {
        /// <summary>
        /// Loads Theme CSS and returns Theme or null if failed.
        /// </summary>
        public Theme LoadCss(string css, string cssAssetName) {

            var parser = new CssParser(css);
            List<CssParser.Selectors> parsedSelectors;

            try
            {
                parsedSelectors = parser.ParseCss();
            }
            catch (CssParseException e)
            {
                Debug.LogErrorFormat("[MarkLight] {0}: Error parsing theme CSS. {1}", cssAssetName, e.Message);
                return null;
            }

            var styleDataList = new List<StyleData>(parsedSelectors.Count);
            var styleDataDict = new Dictionary<string, StyleData>(parsedSelectors.Count);
            var themeProperties = new Dictionary<string, string>();
            var index = 0;

            for (var i = 0; i < parsedSelectors.Count; i++)
            {
                // multiple selectors for a given set of properties (possibly)
                var selectors = parsedSelectors[i];

                // check for theme selector
                if (selectors.SelectorList.Count == 1 && selectors.SelectorList[0].Raw == "Theme")
                {
                    foreach (var prop in selectors.PropertyList)
                    {
                        themeProperties.Add(prop.Name, prop.Value);
                    }
                    continue;
                }

                // get styles from each selector
                foreach (var selector in selectors.SelectorList)
                {
                    var properties = new List<StyleProperty>();

                    // get properties
                    foreach (var prop in selectors.PropertyList)
                    {
                        properties.Add(CreateProperty(selector.Raw, prop));
                    }

                    StyleData prev = null;

                    var combinateCount = selector.Selectors.Count;

                    for (var j = 0; j < combinateCount; j++)
                    {
                        var current = selector.Selectors[j];
                        var combinator = selector.Combinators[j];
                        var style = new StyleSelector(current, combinator);
                        var key = selector + ":" + j;

                        StyleData data;
                        if (!styleDataDict.TryGetValue(key, out data))
                        {
                            data = new StyleData(index++, prev == null ? -1 : prev.Index,
                                style.ElementName, style.Id, style.ClassName, combinator);

                            styleDataDict.Add(key, data);
                            styleDataList.Add(data);
                        }

                        if (j == combinateCount - 1)
                        {
                            data.Properties.RemoveAll(x => properties.Contains(x));
                            data.Properties.AddRange(properties);
                        }

                        prev = data;
                    }
                }
            }

            if (themeProperties.Count == 0)
            {
                Debug.LogErrorFormat("[MarkLight] {0}: Error parsing theme CSS. Missing Theme selector.", cssAssetName);
                return null;
            }

            var themeName = themeProperties.Get("Name");
            if (themeName == null)
            {
                Debug.LogErrorFormat("[MarkLight] {0}: Error parsing theme CSS. Missing Theme Name.", cssAssetName);
                return null;
            }

            var unitSize = themeProperties.Get("UnitSize");
            var baseDirectory = themeProperties.Get("BaseDirectory");
            var hexColorType = themeProperties.Get("HexColorType");

            return new Theme(themeName, baseDirectory, ParseUnitSize(unitSize, cssAssetName),
                                    ParseHexColorType(hexColorType, cssAssetName), styleDataList.ToArray());
        }

        private static StyleProperty CreateProperty(string selector, CssParser.Property cssProperty) {

            var pathIndex = selector.IndexOf("::", StringComparison.Ordinal);
            if (pathIndex == -1)
                return new StyleProperty(cssProperty.Name, cssProperty.Value);

            var path = selector.Substring(pathIndex + 2);
            var name = cssProperty.Name;
            var state = "";

            var stateIndex = name.IndexOf('-', 0);
            if (stateIndex > 0)
            {
                var stateViewField = name.Substring(stateIndex + 1);
                state = name.Substring(0, stateIndex) + '-';
                name = stateViewField;

                var isSubState = name.StartsWith("-");
                if (isSubState)
                {
                    name = name.Substring(1);
                    state += '-';
                }
            }

            name = state + path + '.' + name;
            return new StyleProperty(name, cssProperty.Value);
        }

        private static HexColorType? ParseHexColorType(string hexColorType, string xumlAssetName)
        {
            if (string.IsNullOrEmpty(hexColorType))
                return null;

            var result = EnumValueConverter.HexColorType.Convert(hexColorType);
            if (result.Success)
                return (HexColorType) result.ConvertedValue;

            Debug.LogError(string.Format(
                "[MarkLight] {0}: Error parsing theme CSS. Unable to parse HexColorType attribute value \"{1}\".",
                xumlAssetName, hexColorType));

            return null;
        }

        private static Vector3? ParseUnitSize(string unitSize, string xumlAssetName)
        {
            if (string.IsNullOrEmpty(unitSize))
                return null;

            var converter = new Vector3ValueConverter();
            var result = converter.Convert(unitSize);

            if (result.Success)
                return (Vector3) result.ConvertedValue;

            Debug.LogError(string.Format(
                "[MarkLight] {0}: Error parsing theme CSS. Unable to parse UnitSize attribute value \"{1}\".",
                xumlAssetName, unitSize));

            return null;
        }
    }
}