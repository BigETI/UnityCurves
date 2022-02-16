using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Unity Curves editor namespace
/// </summary>
namespace UnityCurvesEditor
{
    /// <summary>
    /// A class that provides a function that provides curves settings
    /// </summary>
    internal static class CurvesSettingsProvider
    {
        /// <summary>
        /// Scroll view position
        /// </summary>
        public static Vector2 ScrollViewPosition { get; private set; }

        /// <summary>
        /// Creates a curves settings provider
        /// </summary>
        /// <returns>Curves settings provider</returns>
        [SettingsProvider]
        public static SettingsProvider CreateCurvesSettingsProvider()
        {
            SettingsProvider ret = new SettingsProvider("Preferences/Curves", SettingsScope.User)
            {
                guiHandler = (searchContext) =>
                {
                    CurvesSettingsScriptableSingleton settings = CurvesSettingsScriptableSingleton.instance;
                    if (settings)
                    {
                        ScrollViewPosition = EditorGUILayout.BeginScrollView(ScrollViewPosition);
                        settings.PrimaryFontColor = EditorGUILayout.ColorField(new GUIContent("Primary font color"), settings.PrimaryFontColor, true, false, false);
                        settings.SecondaryFontColor = EditorGUILayout.ColorField(new GUIContent("Secondary font color"), settings.SecondaryFontColor, true, false, false);
                        settings.KeyHandleCap = (EHandleCap)EditorGUILayout.EnumPopup("Key handle cap", settings.KeyHandleCap);
                        settings.KeyColor = EditorGUILayout.ColorField("Key color", settings.KeyColor);
                        settings.KeyHandleSizeMultiplier = EditorGUILayout.FloatField("Key handle size multiplier", settings.KeyHandleSizeMultiplier);
                        settings.PrimaryKeyTangentColor = EditorGUILayout.ColorField("Key color", settings.PrimaryKeyTangentColor);
                        settings.SecondaryKeyTangentColor = EditorGUILayout.ColorField("Key color", settings.SecondaryKeyTangentColor);
                        EditorGUILayout.EndScrollView();
                    }
                },
                keywords = new HashSet<string>
                {
                    "Curve",
                    "Bezier",
                    "Bézier"
                }
            };
            return ret;
        }
    }
}
