﻿using UnityEngine;
using UnityPatternsEditor;

/// <summary>
/// Unity Curves editor namespace
/// </summary>
namespace UnityCurvesEditor
{
    /// <summary>
    /// An interface that represents a curves settings scriptable singleton
    /// </summary>
    public interface ICurvesSettingsScriptableSingleton : IScriptableSingleton<CurvesSettingsScriptableSingleton>
    {
        /// <summary>
        /// Curves color
        /// </summary>
        Color CurvesColor { get; set; }

        /// <summary>
        /// Curve line thickness
        /// </summary>
        float CurveLineThickness { get; set; }

        /// <summary>
        /// Primary font color
        /// </summary>
        Color32 PrimaryFontColor { get; set; }

        /// <summary>
        /// Secondary font color
        /// </summary>
        Color32 SecondaryFontColor { get; set; }

        /// <summary>
        /// Handle cap color
        /// </summary>
        EHandleCap KeyHandleCap { get; set; }

        /// <summary>
        /// Key color
        /// </summary>
        Color KeyColor { get; set; }

        /// <summary>
        /// Key handle size multiplier
        /// </summary>
        float KeyHandleSizeMultiplier { get; set; }

        /// <summary>
        /// Primary key tangent color
        /// </summary>
        Color PrimaryKeyTangentColor { get; set; }

        /// <summary>
        /// Secondary key tangent color
        /// </summary>
        Color SecondaryKeyTangentColor { get; set; }
    }
}