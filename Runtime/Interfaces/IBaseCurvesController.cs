using UnityCurves.Data;
using UnityPatterns;

/// <summary>
/// Unity Curves namespace
/// </summary>
namespace UnityCurves
{
    /// <summary>
    /// An interface that represents a base curves controller
    /// </summary>
    public interface IBaseCurvesController : IController
    {
        /// <summary>
        /// Path key segment count
        /// </summary>
        uint PathKeySegmentCount { get; set; }

        /// <summary>
        /// Progress wrap mode
        /// </summary>
        EProgressWrapMode ProgressWrapMode { get; set; }

        /// <summary>
        /// Is interpolating forward vector
        /// </summary>
        bool IsInterpolatingForwardVector { get; set; }

        /// <summary>
        /// Path
        /// </summary>
        BezierCurveData Path { get; set; }

        /// <summary>
        /// Baked target path key segments
        /// </summary>
        IApproximateBezierCurveKeySegments BakedPathKeySegments { get; }

        /// <summary>
        /// Gets Bézier curve object state
        /// </summary>
        /// <param name="progress">Progress</param>
        /// <returns>Bézier curve object state</returns>
        IApproximateBezierCurveObjectState GetBezierCurveObjectState(float progress);
    }
}
