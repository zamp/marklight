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

            var rootStyleList = new List<StyleData>(parsedSelectors.Count);
            var themeProperties = new Dictionary<string, string>();
            var index = 0;

            for (var i = 0; i < parsedSelectors.Count; i++)
            {
                var selector = parsedSelectors[i];

                // check for theme selector
                if (selector.SelectorList.Count == 1 && selector.SelectorList[0].Raw == "Theme")
                {
                    foreach (var prop in selector.PropertyList)
                    {
                        themeProperties.Add(prop.Name, prop.Value);
                    }
                    continue;
                }

                // get styles
                foreach (var sel in selector.SelectorList)
                {
                    var properties = new List<StyleProperty>();

                    // get properties
                    foreach (var prop in selector.PropertyList)
                    {
                        properties.Add(CreateProperty(sel.Raw, prop));
                    }

                    StyleData prev = null;

                    var selectorCount = sel.Selectors.Count;
                    var styleDataDict = new Dictionary<string, StyleData>();

                    for (var j = 0; j < selectorCount; j++)
                    {
                        var curr = sel.Selectors[j];
                        var combinator = sel.Combinators[j];
                        var style = new StyleSelector(curr, combinator);
                        var key = sel.Raw + ":" + style.LocalSelector;

                        StyleData data;
                        if (!styleDataDict.TryGetValue(key, out data))
                        {
                            data = new StyleData(index++, prev == null ? -1 : prev.Index,
                                                 style.ElementName, style.Id, style.ClassName, combinator);

                            styleDataDict.Add(key, data);
                            rootStyleList.Add(data);
                        }

                        if (j == selectorCount - 1)
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
            var isUnitSizeSet = unitSize != null;
            var isBaseDirectorySet = baseDirectory != null;

            return new Theme(themeName, baseDirectory, ParseUnitSize(unitSize, cssAssetName),
                                    isBaseDirectorySet, isUnitSizeSet, rootStyleList.ToArray());
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

        private static Vector3 ParseUnitSize(string unitSize, string cssAssetName) {
            if (string.IsNullOrEmpty(unitSize))
            {
                // use default unit size
                return ViewPresenter.Instance.UnitSize;
            }
            var converter = new Vector3ValueConverter();
            var result = converter.Convert(unitSize);
            if (result.Success)
            {
                return (Vector3) result.ConvertedValue;
            }

            Debug.LogError(string.Format(
                "[MarkLight] {0}: Error parsing theme CSS. Unable to parse UnitSize attribute value \"{1}\".",
                cssAssetName, unitSize));

            return ViewPresenter.Instance.UnitSize;
        }
    }
}