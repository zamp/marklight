using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MarkLight
{
    /// <summary>
    /// Extension methods.
    /// </summary>
    public static class ExtensionMethods
    {
        #region Methods

        /// <summary>
        /// Destroys a view.
        /// </summary>
        public static void Destroy(this View view, bool immediate = false)
        {
            view.IsDestroyed.DirectValue = true;
            if (view.LayoutParent != null)
            {
                view.LayoutParent.LayoutChildren.Remove(view);
            }
            if (Application.isPlaying && !immediate)
            {
                Object.Destroy(view.gameObject);
            }
            else
            {
                Object.DestroyImmediate(view.gameObject);
            }
        }

        /// <summary>
        /// Destroy a view or moves it back into view pool.
        /// </summary>
        public static void Destroy(this View view, ViewPool viewPool, bool immediate = false)
        {
            if (viewPool == null || viewPool.IsFull)
            {
                view.Destroy(immediate);
                return;
            }

            // move view into view pool
            viewPool.InsertView(view);
        }

        /// <summary>
        /// Destroys all children of a view.
        /// </summary>
        public static void DestroyChildren(this View view, bool immediate = false)
        {
            var childCount = view.LayoutChildren.Count;
            for (var i = childCount - 1; i >= 0; --i)
            {
                var childView = view.LayoutChildren[i];
                if (childView == null)
                    continue;

                childView.IsDestroyed.DirectValue = true;
                var go = childView.gameObject;

                view.LayoutChildren.RemoveAt(i);

                if (Application.isPlaying && !immediate)
                {
                    Object.Destroy(go);
                }
                else
                {
                    Object.DestroyImmediate(go);
                }
            }
        }

        /// <summary>
        /// Checks if a flag enum has a flag value set.
        /// </summary>
        public static bool HasFlag(this Enum variable, Enum value)
        {
            // check if from the same type
            if (variable.GetType() != value.GetType())
            {
                Debug.LogError("[MarkLight] The checked flag is not from the same type as the checked variable.");
                return false;
            }

            var num = Convert.ToUInt64(value);
            var num2 = Convert.ToUInt64(variable);
            return (num2 & num) == num;
        }

        /// <summary>
        /// Clamps a value to specified range [min, max].
        /// </summary>
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T> {
            if (val.CompareTo(min) < 0) return min;
            return val.CompareTo(max) > 0 ? max : val;
        }

        /// <summary>
        /// Gets value from dictionary and returns null if it doesn't exist.
        /// </summary>
        public static TValue Get<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key) {
            TValue value;
            return !dict.TryGetValue(key, out value) ? default(TValue) : value;
        }

        /// <summary>
        /// Resets a rect transform.
        /// </summary>
        public static void Reset(this RectTransform rectTransform)
        {
            rectTransform.localScale = new Vector3(1f, 1f, 1f);
            rectTransform.localPosition = new Vector3(0f, 0f, 0f);
            rectTransform.anchorMin = new Vector2(0f, 0f);
            rectTransform.anchorMax = new Vector2(1f, 1f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.offsetMin = new Vector2(0.0f, 0.0f);
            rectTransform.offsetMax = new Vector2(0.0f, 0.0f);
        }

        /// <summary>
        /// Calculates mouse screen position.
        /// </summary>
        public static Vector2 GetMouseScreenPosition(this UnityEngine.Canvas canvas, Vector3 mousePosition)
        {
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, mousePosition,
                canvas.worldCamera, out pos);
            Vector2 mouseScreenPosition = canvas.transform.TransformPoint(pos);
            return mouseScreenPosition;
        }

        /// <summary>
        /// Calculates mouse screen position.
        /// </summary>
        public static Vector2 GetMouseScreenPosition(this UnityEngine.Canvas canvas, Vector2 mousePosition)
        {
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, mousePosition,
                canvas.worldCamera, out pos);
            Vector2 mouseScreenPosition = canvas.transform.TransformPoint(pos);
            return mouseScreenPosition;
        }

        /// <summary>
        /// Removes all whitespace from a string.
        /// </summary>
        public static string RemoveWhitespace(this string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
        }

        /// <summary>
        /// Gets view field info from a type.
        /// </summary>
        public static MemberInfo GetFieldInfo(this Type type, string field,
                                              BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance)
        {
            var fieldInfo = type.GetField(field, bindingFlags);
            if (fieldInfo != null)
                return fieldInfo;

            var propertyInfo = type.GetProperty(field, bindingFlags);
            return propertyInfo;
        }

        /// <summary>
        /// Gets view field type from view field info.
        /// </summary>
        public static Type GetFieldType(this MemberInfo memberInfo)
        {
            var fieldInfo = memberInfo as FieldInfo;
            if (fieldInfo != null)
            {
                return fieldInfo.FieldType;
            }

            var propertyInfo = memberInfo as PropertyInfo;
            if (propertyInfo != null)
            {
                return propertyInfo.PropertyType;
            }

            return null;
        }

        /// <summary>
        /// Gets value from a view field.
        /// </summary>
        public static object GetFieldValue(this MemberInfo memberInfo, object typeObject)
        {
            var fieldInfo = memberInfo as FieldInfo;
            if (fieldInfo != null)
                return fieldInfo.GetValue(typeObject);

            var propertyInfo = memberInfo as PropertyInfo;
            return propertyInfo.GetValue(typeObject, null);
        }

        /// <summary>
        /// Sets view field value.
        /// </summary>
        public static void SetFieldValue(this MemberInfo memberInfo, object typeObject, object value)
        {
            var fieldInfo = memberInfo as FieldInfo;
            if (fieldInfo != null)
            {
                fieldInfo.SetValue(typeObject, value);
                return;
            }

            var propertyInfo = memberInfo as PropertyInfo;
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(typeObject, value, null);
            }
        }

        /// <summary>
        /// Adds range of items to a hashset.
        /// </summary>
        public static void AddRange<T>(this HashSet<T> hashSet, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                hashSet.Add(item);
            }
        }

#if !UNITY_4_6 && !UNITY_5_0 && !UNITY_5_1
        /// <summary>
        /// Converts panel scrollbar visibility to unity scrollrect scrollbar visibility.
        /// </summary>
        public static UnityEngine.UI.ScrollRect.ScrollbarVisibility ToScrollRectVisibility(
                                                                            this PanelScrollbarVisibility visibility)
        {
            switch (visibility)
            {
                case PanelScrollbarVisibility.Permanent:
                    return UnityEngine.UI.ScrollRect.ScrollbarVisibility.Permanent;
                case PanelScrollbarVisibility.AutoHide:
                case PanelScrollbarVisibility.Hidden:
                case PanelScrollbarVisibility.Remove:
                    return UnityEngine.UI.ScrollRect.ScrollbarVisibility.AutoHide;
                case PanelScrollbarVisibility.AutoHideAndExpandViewport:
                    return UnityEngine.UI.ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport;
                default:
                    throw new ArgumentOutOfRangeException("visibility", visibility, null);
            }
        }
#endif

        /// <summary>
        /// Converts content alignment to pivot.
        /// </summary>
        public static Vector2 ToPivot(this ElementAlignment alignment)
        {
            switch (alignment)
            {
                case ElementAlignment.Center:
                    return new Vector2(0.5f, 0.5f);
                case ElementAlignment.Left:
                    return new Vector2(0, 0.5f);
                case ElementAlignment.Top:
                    return new Vector2(0.5f, 1);
                case ElementAlignment.Right:
                    return new Vector2(1, 0.5f);
                case ElementAlignment.Bottom:
                    return new Vector2(0.5f, 0);
                case ElementAlignment.TopLeft:
                    return new Vector2(0, 1);
                case ElementAlignment.TopRight:
                    return new Vector2(1, 1);
                case ElementAlignment.BottomLeft:
                    return new Vector2(0, 0);
                case ElementAlignment.BottomRight:
                    return new Vector2(1, 0);
                default:
                    throw new ArgumentOutOfRangeException("alignment", alignment, null);
            }
        }

        /// <summary>
        /// Replaces the first occurance of a string.
        /// </summary>
        public static string ReplaceFirst(this string text, string search, string replace)
        {
            var pos = text.IndexOf(search, StringComparison.Ordinal);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        #endregion
    }
}
