using UnityEngine;

/// <summary>
/// Unity Curves namespace
/// </summary>
namespace UnityCurves
{
    /// <summary>
    /// A class that describes an approximate bezier curve key segment
    /// </summary>
    public readonly struct ApproximateBezierCurveKeySegment : IApproximateBezierCurveKeySegment
    {
        /// <summary>
        /// Length
        /// </summary>
        public float Length { get; }

        /// <summary>
        /// Forward vector
        /// </summary>
        public Vector3 Forward { get; }
        
        /// <summary>
        /// Up vector angle in degrees
        /// </summary>
        public float UpVectorAngle { get; }

        /// <summary>
        /// Delta
        /// </summary>
        public Vector3 Delta { get; }

        /// <summary>
        /// Constructs a new approximate bezier curve key segment
        /// </summary>
        /// <param name="length">Length</param>
        /// <param name="forward">Forward vector</param>
        /// <param name="upVectorAngle">Up vector angle in degrees</param>
        /// <param name="delta">Delta</param>
        public ApproximateBezierCurveKeySegment(float length, Vector3 forward, float upVectorAngle, Vector3 delta)
        {
            Length = length;
            Forward = forward;
            UpVectorAngle = upVectorAngle;
            Delta = delta;
        }
    }
}
