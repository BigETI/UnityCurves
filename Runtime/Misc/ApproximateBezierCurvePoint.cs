using UnityEngine;

/// <summary>
/// Unity Curves namespace
/// </summary>
namespace UnityCurves
{
    /// <summary>
    /// A class that describes an approximate bezier curve point
    /// </summary>
    public readonly struct ApproximateBezierCurvePoint : IApproximateBezierCurvePoint
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
        /// Up vector
        /// </summary>
        public Vector3 Up { get; }
        
        /// <summary>
        /// Progress
        /// </summary>
        public float Progress { get; }

        /// <summary>
        /// Constructs a new approximate bezier curve object state
        /// </summary>
        /// <param name="position">Position</param>
        /// <param name="forward">Forward vector</param>
        /// <param name="up">Up vector</param>
        /// <param name="progress">Progress</param>
        public ApproximateBezierCurvePoint(Vector3 position, Vector3 forward, Vector3 up, float progress)
        {
            Position = position;
            Forward = forward;
            Up = up;
            Progress = progress;
        }
    }
}
