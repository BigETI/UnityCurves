using UnityEngine;

/// <summary>
/// Unity Curves namespace
/// </summary>
namespace UnityCurves
{
    /// <summary>
    /// An interface that represents an approximate bezier curve object state
    /// </summary>
    public interface IApproximateBezierCurveObjectState
    {
        /// <summary>
        /// Position
        /// </summary>
        Vector3 Position { get; }

        /// <summary>
        /// Forward vector
        /// </summary>
        Vector3 Forward { get; }
    }
}
