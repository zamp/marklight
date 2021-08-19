﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MarkLight.ValueConverters;
using UnityEngine;

namespace MarkLight
{
    /// <summary>
    /// Contains information about a view field path.
    /// </summary>
    public class ViewFieldPathInfo
    {
        #region Fields

        public readonly Type SourceViewType;
        public readonly string Path;
        public readonly string[] Fields;
        public readonly string RootFieldName;

        private static readonly Type ViewFieldBaseType = typeof(ViewFieldBase);
        private static readonly Type ItemViewFieldType = typeof(ItemViewField);
        private static readonly Type ViewType = typeof(View);

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        public ViewFieldPathInfo(View sourceView, string path)
        {
            SourceViewType = sourceView.GetType();
            Path = path;
            TargetPath = path;
            TypeName = SourceViewType.Name;
            Fields = path.Split('.');
            RootFieldName = Fields[0];
            MemberInfo = new List<MemberInfo>();
            Dependencies = new HashSet<string>();

            Init(sourceView);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get the view that is targeted by the path.
        /// </summary>
        public View GetTargetView(View sourceView)
        {
            if (IsMapped)
            {
                var view = GetRootFieldView(sourceView);
                if (view != null)
                    return view;
            }

            return !IsMappedToDescendants
                ? sourceView
                : sourceView.Find<View>(x => x.Id == RootFieldName, new ViewSearchArgs(true)
                    {
                        Parent = sourceView
                    });
        }

        /// <summary>
        /// Gets value at the end of the field path.
        /// </summary>
        public object GetValue(View sourceView, out bool hasValue)
        {
            hasValue = true;
            object viewFieldObject = sourceView;
            for (var i=0; i < MemberInfo.Count; i++)
            {
                var memberInfo = MemberInfo[i];
                var isLast = i == MemberInfo.Count - 1;

                viewFieldObject = memberInfo.GetFieldValue(viewFieldObject);
                if (viewFieldObject != null || isLast)
                    continue;

                hasValue = false;
                return null;
            }

            return viewFieldObject;
        }

        /// <summary>
        /// Sets value and returns old value.
        /// </summary>
        public object SetValue(View sourceView, object value)
        {
            object viewFieldObject = sourceView;
            for (var i = 0; i < MemberInfo.Count; ++i)
            {
                var lastMemberInfo = i == MemberInfo.Count - 1;

                var memberInfo = MemberInfo[i];
                                
                if (lastMemberInfo)
                {
                    var oldValue = memberInfo.GetFieldValue(viewFieldObject);

                    // set value
                    memberInfo.SetFieldValue(viewFieldObject, value);
                    return oldValue;
                }

                viewFieldObject = memberInfo.GetFieldValue(viewFieldObject);                
                if (viewFieldObject == null)
                    return null;
            }

            return null;
        }

        /// <summary>
        /// Tries to parse the view field path and get view field path info.
        /// Called only if the field is not mapped to or through a descendant view.
        /// </summary>
        public bool ParsePath(View sourceView)
        {
            IsSevereParseError = false;

            IsParsed = false;
            MemberInfo.Clear();
            Dependencies.Clear();

            var viewTypeData = sourceView.ViewTypeData;

            // if we get here we are the owner of the field and need to parse the path
            ValueConverter = viewTypeData.GetViewFieldValueConverter(Path);

            object viewFieldObject = sourceView;

            // parse view field path
            var isParseSuccess = true;
            var dependencyPath = string.Empty;
            for (var i = 0; i < Fields.Length; ++i)
            {
                var isLastField = i == Fields.Length - 1;
                var viewField = Fields[i];

                // add dependency
                if (!isLastField)
                {
                    dependencyPath += (i > 0 ? "." : "") + viewField;
                    Dependencies.Add(dependencyPath);
                }

                if (!isParseSuccess)
                    continue;

                var viewFieldType = viewFieldObject.GetType();
                var memberInfo = viewFieldType.GetFieldInfo(viewField);
                if (memberInfo == null)
                {
                    IsSevereParseError = true;
                    Utils.ErrorMessage = String.Format(
                        "Unable to parse view field path \"{0}\". Couldn't find member with the name \"{1}\".", Path,
                        viewField);
                    return false;
                }

                MemberInfo.Add(memberInfo);
                Type = memberInfo.GetFieldType();

                if (i == 0)
                {
                    IsDataModelItem = ItemViewFieldType.IsAssignableFrom(Type);
                }

                // handle special ViewFieldBase types
                if (ViewFieldBaseType.IsAssignableFrom(Type))
                {
                    viewFieldObject = memberInfo.GetFieldValue(viewFieldObject);
                    if (viewFieldObject == null)
                    {
                        Utils.ErrorMessage = String.Format(
                            "Unable to parse view field path \"{0}\". Field/property with the name \"{1}\" was null.",
                            Path, viewField);
                        isParseSuccess = false;
                        continue;
                    }

                    memberInfo = Type.GetProperty("InternalValue"); // set internal dependency view field value
                    MemberInfo.Add(memberInfo);
                    Type = memberInfo.GetFieldType();
                }

                if (isLastField)
                {
                    Type = memberInfo.GetFieldType();
                    TypeName = Type.Name;
                    ValueConverter = ValueConverter ?? ViewData.GetValueConverterForType(TypeName);

                    // handle special case if converter is null
                    if (ValueConverter == null)
                    {
                        if (Type.IsEnum())
                        {
                            // if enum use generic enum converter
                            ValueConverter = new EnumValueConverter(Type);
                        }
                        else if (Type.IsSubclassOf(typeof(Component)))
                        {
                            // if component use generic component converter
                            ValueConverter = new ComponentValueConverter(Type);
                        }
                    }
                }
                else
                {
                    viewFieldObject = memberInfo.GetFieldValue(viewFieldObject);
                }

                if (viewFieldObject != null)
                    continue;

                Utils.ErrorMessage = String.Format(
                    "Unable to parse view field path \"{0}\". Field/property with the name \"{1}\" was null.", Path,
                    viewField);
                isParseSuccess = false;
            }

            IsParsed = isParseSuccess;

            return isParseSuccess;
        }

        /// <summary>
        /// Get the root field path as a View.
        /// </summary>
        public View GetRootFieldView(View sourceView)
        {
            return MemberInfo.Count == 0
                ? null
                : MemberInfo[0].GetFieldValue(sourceView) as View;
        }

        private void Init(View sourceView)
        {

            // do we have a view field path consisting of multiple view fields?
            if (Fields.Length == 1)
            {
                // Field is local so its safe to parse because the parsed fields will be the
                // same for all of the view type instances.
                ParsePath(sourceView);
                return;
            }

            // is this a field that refers to another view?
            var fieldInfo = SourceViewType.GetField(RootFieldName);
            if (fieldInfo != null && ViewType.IsAssignableFrom(fieldInfo.FieldType))
            {
                // yes. set target view and return
                TargetPath = String.Join(".", Fields.Skip(1).ToArray());
                MemberInfo.Add(fieldInfo);
                IsMapped = true;
                IsParsed = true;
                InitDependencies();
                return;
            }

            // is this a property that refers to a view?
            var propertyInfo = SourceViewType.GetProperty(RootFieldName);
            if (propertyInfo != null && ViewType.IsAssignableFrom(propertyInfo.PropertyType))
            {
                // yes. set target view and return
                TargetPath = String.Join(".", Fields.Skip(1).ToArray());
                MemberInfo.Add(propertyInfo);
                IsMapped = true;
                IsParsed = true;
                InitDependencies();
                return;
            }

            // does first view field or property exist?
            if (fieldInfo == null && propertyInfo == null)
            {
                TargetPath = String.Join(".", Fields.Skip(1).ToArray());
                IsMapped = true;
                IsMappedToDescendants = true;
                InitDependencies();
            }
        }

        private void InitDependencies() {

            Dependencies.Clear();

            var dependencyPath = string.Empty;
            for (var i = 0; i < Fields.Length; ++i)
            {
                var isLastField = i == Fields.Length - 1;
                var viewField = Fields[i];

                // add dependency
                if (isLastField)
                    continue;

                dependencyPath += (i > 0 ? "." : "") + viewField;
                Dependencies.Add(dependencyPath);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Get path to target field.
        /// </summary>
        public string TargetPath
        {
            get; private set;
        }

        /// <summary>
        /// Get list of member info.
        /// </summary>
        public List<MemberInfo> MemberInfo
        {
            get; private set;
        }

        /// <summary>
        /// Get list of dependenciy field names.
        /// </summary>
        public HashSet<string> Dependencies
        {
            get; private set;
        }

        /// <summary>
        /// Get the value converter to use for the field.
        /// </summary>
        public ValueConverter ValueConverter
        {
            get; private set;
        }

        /// <summary>
        /// Get the field type name.
        /// </summary>
        public string TypeName
        {
            get; private set;
        }

        /// <summary>
        /// Get the field data type.
        /// </summary>
        public Type Type
        {
            get; private set;
        }

        /// <summary>
        /// Determine if the field is mapped.
        /// </summary>
        public bool IsMapped
        {
            get; private set;
        }

        /// <summary>
        /// Determine if the field is mapped to or through a descendant view.
        /// </summary>
        public bool IsMappedToDescendants
        {
            get; private set;
        }

        /// <summary>
        /// Determine if the field path is parsed.
        /// </summary>
        public bool IsParsed
        {
            get; private set;
        }

        /// <summary>
        /// Determine if the field is for data model item.
        /// </summary>
        public bool IsDataModelItem
        {
            get; private set;
        }

        /// <summary>
        /// Determine if the last parse failure was a severe error.
        /// </summary>
        public bool IsSevereParseError
        {
            get; private set;
        }

        #endregion
    }
}
