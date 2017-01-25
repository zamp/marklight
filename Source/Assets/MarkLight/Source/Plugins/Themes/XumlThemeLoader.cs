using System;
using System.Collections.Generic;
using System.Xml.Linq;
using MarkLight;
using MarkLight.ValueConverters;
using UnityEngine;

namespace Marklight.Themes
{
    public class XumlThemeLoader
    {
        /// <summary>
        /// Loads Theme XUML and returns Theme or null if failed.
        /// </summary>
        public Theme LoadXuml(XElement rootElement, string xuml, string xumlAssetName)
        {
            var themeNameAttr = rootElement.Attribute("Name");
            if (themeNameAttr == null)
            {
                Debug.LogErrorFormat("[MarkLight] {0}: Error parsing theme XUML. Name attribute missing.", xumlAssetName);
                return null;
            }

            var baseDirectoryAttr = rootElement.Attribute("BaseDirectory");
            var unitSizeAttr = rootElement.Attribute("UnitSize");
            var hasBaseDirectory = baseDirectoryAttr != null;
            var hasUnitSize = unitSizeAttr != null;

            var baseDirectory = baseDirectoryAttr != null ? baseDirectoryAttr.Value : "";
            var unitSize = ParseUnitSize(hasUnitSize ? unitSizeAttr.Value : null, xumlAssetName);

            try
            {
                var styles = LoadStyles(rootElement, new LoadStyleContext(), -1, new List<StyleData>(50));

                return new Theme(themeNameAttr.Value,
                    baseDirectory, unitSize, hasBaseDirectory, hasUnitSize, styles.ToArray());
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("[MarkLight] {0}: Error parsing theme XUML. {1}", xumlAssetName, e);
                return null;
            }
        }

        private class LoadStyleContext
        {
            public int StyleIndex = -1;
        }

        private List<StyleData> LoadStyles(XContainer parent,
                                       LoadStyleContext context, int parentIndex, List<StyleData> output) {

            // load theme elements
            foreach (var element in parent.Elements())
            {
                context.StyleIndex++;

                var idAttr = element.Attribute("Id");
                var classNameAttr = element.Attribute("Style");
                var basedOnAttr = element.Attribute("BasedOn");

                string elementName = null;
                string id = null;
                string className = null;
                string basedOn = null;


                if (element.Name.LocalName != "Style")
                    elementName = element.Name.LocalName;

                if (idAttr != null)
                    id = idAttr.Value;

                if (classNameAttr != null)
                    className = classNameAttr.Value;

                if (basedOnAttr != null)
                    basedOn = basedOnAttr.Value;

                var styleData = new StyleData(context.StyleIndex, parentIndex, elementName, id, className, basedOn);
                var properties = LoadAttributes(element);

                var existingIndex = output.IndexOf(styleData);
                if (existingIndex != -1)
                {
                    var existing = output[existingIndex];

                    foreach (var property in styleData.Properties)
                    {
                        existing.Properties.Remove(property);
                        existing.Properties.Add(property);
                    }
                }
                else
                {
                    styleData.Properties.AddRange(properties);
                    output.Add(styleData);
                }

                LoadStyles(element, context, styleData.Index, output);
            }

            return output;
        }

        private static IEnumerable<StyleProperty> LoadAttributes(XElement xumlElement) {

            var result = new List<StyleProperty>(10);

            foreach (var attribute in xumlElement.Attributes())
            {
                var name = attribute.Name.LocalName;
                var value = attribute.Value;

                // ignore namespace specification
                if (string.Equals(name, "xmlns", StringComparison.OrdinalIgnoreCase))
                    continue;

                // ignore id specification
                if (string.Equals(name, "id", StringComparison.OrdinalIgnoreCase))
                    continue;

                // ignore basedon specification
                if (string.Equals(name, "basedon", StringComparison.OrdinalIgnoreCase))
                    continue;

                // ignore classname specification
                if (string.Equals(name, "classname", StringComparison.OrdinalIgnoreCase))
                    continue;

                // ignore obsolete style view field
                if (string.Equals(name, "style", StringComparison.OrdinalIgnoreCase))
                    continue;

                result.Add(new StyleProperty(name, value));
            }

            return result;
        }

        private static Vector3 ParseUnitSize(string unitSize, string xumlAssetName) {
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
                "[MarkLight] {0}: Error parsing theme XUML. Unable to parse UnitSize attribute value \"{1}\".",
                xumlAssetName, unitSize));

            return ViewPresenter.Instance.UnitSize;
        }
    }
}