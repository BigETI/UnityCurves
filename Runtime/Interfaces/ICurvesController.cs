﻿using UnityCurves.Data;
using UnityEngine;
using UnityPatterns;

/// <summary>
/// Unity Curves namespace
/// </summary>
namespace UnityCurves
{
    /// <summary>
    /// An interface that represents a curves controller
    /// </summary>
    public interface ICurvesController : IController
    {
        /// <summary>
        /// Path key segment count
        /// </summary>
        uint PathKeySegmentCount { get; set; }

        /// <summary>
        /// Progress
        /// </summary>
        float Progress { get; set; }

        /// <summary>
        /// Progress wrap mode
        /// </summary>
        EProgressWrapMode ProgressWrapMode { get; set; }

        /// <summary>
        /// Is interpolating forward vector
        /// </summary>
        bool IsInterpolatingForwardVector { get; set; }

        /// <summary>
        /// Target transform
        /// </summary>
        Transform TargetTransform { get; set; }

        /// <summary>
        /// Target path
        /// </summary>
        BezierCurveData TargetPath { get; set; }

        /// <summary>
        /// Target state
        /// </summary>
        IApproximateBezierCurveObjectState TargetState { get; }

        /// <summary>
        /// Baked target path key segments
        /// </summary>
        IApproximateBezierCurveKeySegments BakedTargetPathKeySegments { get; }
    }
}