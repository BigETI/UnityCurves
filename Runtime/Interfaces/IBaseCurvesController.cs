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
        /// Gets the approximate bezier curve point in world space
        /// </summary>
        /// <param name="localApproximateBezierCurvePoint">Local approximate bezier curve point</param>
        /// <returns>Approximate bezier curve point in world space</returns>
        IApproximateBezierCurvePoint GetWorldApproximateBezierCurvePoint(IApproximateBezierCurvePoint localApproximateBezierCurvePoint);

        /// <summary>
        /// Gets an approximate Bézier curve point
        /// </summary>
        /// <param name="progress">Progress</param>
        /// <returns>Approximate Bézier curve point</returns>
        IApproximateBezierCurvePoint GetApproximateBezierCurvePoint(float progress);
    }
}
