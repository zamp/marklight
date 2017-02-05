using System;
using System.Collections.Generic;
using Marklight;
using UnityEditor;
using UnityEngine;

namespace MarkLight.Editor
{
    /// <summary>
    /// Custom inspector for ViewPresenter components.
    /// </summary>
    [CustomEditor(typeof(ViewPresenter))]
    public class ViewPresenterInspector : UnityEditor.Editor
    {
        #region Methods

        /// <summary>
        /// Called when inspector GUI is to be rendered.
        /// </summary>
        public override void OnInspectorGUI() {
            //DrawDefaultInspector();

            var presenter = (ViewPresenter) target;

            // main view selection
            var selectedIndex = presenter.ViewTypeNames.IndexOf(presenter.MainView) + 1;

            // .. add empty selection
            var mainOptions = new List<string>(presenter.ViewTypeNames);
            mainOptions.Insert(0, "-- none --");

            // .. add drop-down logic
            var newSelectedIndex = EditorGUILayout.Popup("Main View", selectedIndex, mainOptions.ToArray());

            presenter.MainView = newSelectedIndex > 0
                ? presenter.ViewTypeNames[newSelectedIndex - 1]
                : String.Empty;

            if (newSelectedIndex != selectedIndex)
            {
                // .. trigger reload on view presenter
                if (!presenter.DisableAutomaticReload)
                    ViewData.GenerateViews();
            }

            // default theme selection
            var selectedThemeIndex = presenter.ThemeNames.IndexOf(presenter.DefaultTheme) + 1;

            // .. add empty selection
            var themeOptions = new List<string>(presenter.ThemeNames);
            themeOptions.Insert(0, "-- none --");

            // .. add drop-down logic
            var newSelectedThemeIndex =
                EditorGUILayout.Popup("Default Theme", selectedThemeIndex, themeOptions.ToArray());

            presenter.DefaultTheme = newSelectedThemeIndex > 0
                ? presenter.ThemeNames[newSelectedThemeIndex - 1]
                : String.Empty;

            if (newSelectedThemeIndex != selectedThemeIndex)
            {
                // .. trigger reload on view presenter
                if (!presenter.DisableAutomaticReload)
                    ViewData.GenerateViews();
            }

            // hex color type
            var hexColorTypeContent = new GUIContent("Hex Color Type",
                "The type of hex color codes used when dealing alpha values.");
            var newHexColorType = EditorGUILayout.EnumPopup(hexColorTypeContent, presenter.HexColorType);
            if (!presenter.HexColorType.Equals(newHexColorType))
            {
                presenter.HexColorType = (HexColorType)newHexColorType;
            }

            // unit size option
            var unitSizeContent = new GUIContent("Unit Size",
                "User-defined unit size that can be used in XUML, e.g. Offset=\"12ux, 4uy\"");
            var newUnitSize = EditorGUILayout.Vector3Field(unitSizeContent, presenter.UnitSize);
            if (newUnitSize != presenter.UnitSize)
            {
                presenter.UnitSize = newUnitSize;
            }

            // base directory
            var baseDirectoryContent = new GUIContent("Base Directory",
                "Base directory that will be prepended to assets paths in XUML. E.g. \"Assets/Path/To/Folder/\"");
            var newBaseDirectory = EditorGUILayout.TextField(baseDirectoryContent, presenter.BaseDirectory);
            if (newBaseDirectory != presenter.BaseDirectory)
            {
                presenter.BaseDirectory = newBaseDirectory;
            }

            // default resource dictionary language
            var defaultLanguageContent = new GUIContent("Default Language",
                "Default language to be set on the resource dictionary");
            var defaultLanguage = EditorGUILayout.TextField(defaultLanguageContent, presenter.DefaultLanguage);
            if (defaultLanguage != presenter.DefaultLanguage)
            {
                presenter.DefaultLanguage = defaultLanguage;
            }

            // default resource dictionary platform
            var defaultPlatformContent = new GUIContent("Default Platform",
                "Default platform to be set on the resource dictionary");
            var defaultPlatform = EditorGUILayout.TextField(defaultPlatformContent, presenter.DefaultPlatform);
            if (defaultPlatform != presenter.DefaultPlatform)
            {
                presenter.DefaultPlatform = defaultPlatform;
            }

            // disable automatic reload option
            var disableReloadContent = new GUIContent("Disable Automatic Reload",
                "When checked views are only reloaded when the \"Reload Views\" button is clicked.");
            presenter.DisableAutomaticReload =
                EditorGUILayout.Toggle(disableReloadContent, presenter.DisableAutomaticReload);

            // generate XSD schema
            var generateSchemaContent = new GUIContent("Generate Schema",
                "Generates a new XSD schema for all the views. Used to get intellisense when editing XUML in "+
                "visual studio. Schema need to be generated when new views are added, renamed or new view "+
                "fields are added.");
            if (GUILayout.Button(generateSchemaContent))
            {
                ViewPostprocessor.GenerateXsdSchema();
            }

            // reload button
            var reloadViewsContent = new GUIContent("Reload Views",
                "Reloads the views in the scene. Views are automatically reloaded when XUML changes. "+
                "The views need to be manually reloaded when XUML has been edited while the editor was closed "+
                "or when only code (view models) has been modified.");
            if (GUILayout.Button(reloadViewsContent))
            {
                // .. trigger reload of views
                ViewPostprocessor.ProcessViewAssets();
            }
        }

        #endregion
    }
}
