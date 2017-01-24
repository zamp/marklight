using System;
using UnityEngine;

namespace Marklight.Themes
{
    [Flags]
    public enum StyleSelectorType : byte
    {
        None = 0,
        Element = 1 << 0,
        Id = 1 << 1,
        Class = 1 << 2
    }

    public static class StyleSelectorTypeExtensions
    {
        public static string GetSelectorName(this StyleSelectorType type, string styleName)
        {

            if (string.IsNullOrEmpty(styleName))
                return styleName;

            switch (type)
            {
                case StyleSelectorType.Element:
                    return styleName;
                case StyleSelectorType.Class:
                    return '.' + styleName;
                case StyleSelectorType.Id:
                    return '#' + styleName;
                case StyleSelectorType.None:
                    Debug.LogWarning(styleName);
                    return styleName;
                default:
                    throw new ArgumentOutOfRangeException("type", type, null);
            }
        }

        public static bool IsElement(this StyleSelectorType type)
        {
            return type == StyleSelectorType.Element;
        }

        public static bool IsClass(this StyleSelectorType type)
        {
            return type == StyleSelectorType.Class;
        }

        public static bool IsId(this StyleSelectorType type)
        {
            return type == StyleSelectorType.Id;
        }
    }
}