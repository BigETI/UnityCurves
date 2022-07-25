using UnityEngine;

/// <summary>
/// Unity Curves namespace
/// </summary>
namespace UnityCurves
{
    /// <summary>
    /// An interface that represents an approximate bezier curve point
    /// </summary>
    public interface IApproximateBezierCurvePoint
    {
        /// <summary>
        /// Position
        /// </summary>
        Vector3 Position { get; }

        /// <summary>
        /// Forward vector
        /// </summary>
        Vector3 Forward { get; }

        /// <summary>
        /// Up vector
        /// </summary>
        Vector3 Up { get; }

        /// <summary>
        /// Progress
        /// </summary>
        float Progress { get; }
    }
}
