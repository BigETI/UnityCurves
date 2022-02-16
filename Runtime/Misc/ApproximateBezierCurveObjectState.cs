using UnityEngine;

/// <summary>
/// Unity Curves namespace
/// </summary>
namespace UnityCurves
{
    /// <summary>
    /// A class that describes an approximate bezier curve object state
    /// </summary>
    public readonly struct ApproximateBezierCurveObjectState : IApproximateBezierCurveObjectState
    {
        /// <summary>
        /// Position
        /// </summary>
        public Vector3 Position { get; }

        /// <summary>
        /// Forward vector
        /// </summary>
        public Vector3 Forward { get; }

        /// <summary>
        /// Constructs a new approximate bezier curve object state
        /// </summary>
        /// <param name="position">Position</param>
        /// <param name="forward">Forward</param>
        public ApproximateBezierCurveObjectState(Vector3 position, Vector3 forward)
        {
            Position = position;
            Forward = forward;
        }
    }
}
