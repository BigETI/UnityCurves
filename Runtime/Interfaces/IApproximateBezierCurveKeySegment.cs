using UnityEngine;

/// <summary>
/// Unity Curves namespace
/// </summary>
namespace UnityCurves
{
    /// <summary>
    /// An interface that represents an approximate bezier curve key segment
    /// </summary>
    public interface IApproximateBezierCurveKeySegment
    {
        /// <summary>
        /// Length
        /// </summary>
        float Length { get; }

        /// <summary>
        /// Forward vector
        /// </summary>
        Vector3 Forward { get; }

        /// <summary>
        /// Delta
        /// </summary>
        Vector3 Delta { get; }
    }
}
