using UnityEngine;

/// <summary>
/// Unity Curves namespace
/// </summary>
namespace UnityCurves
{
    /// <summary>
    /// An interface that represents bezier curve key data
    /// </summary>
    public interface IBezierCurveKeyData
    {
        /// <summary>
        /// Start position
        /// </summary>
        Vector3 StartPosition { get; set; }

        /// <summary>
        /// End position
        /// </summary>
        Vector3 EndPosition { get; set; }

        /// <summary>
        /// Start tangent
        /// </summary>
        Vector3 StartTangent { get; set; }

        /// <summary>
        /// End tangent
        /// </summary>
        Vector3 EndTangent { get; set; }

        /// <summary>
        /// Is end position connected
        /// </summary>
        bool IsEndPositionConnected { get; set; }

        /// <summary>
        /// Is end tangent connected
        /// </summary>
        bool IsEndTangentConnected { get; set; }

        /// <summary>
        /// Gets position at the specified time
        /// </summary>
        /// <param name="time">Time from 0 to 1</param>
        /// <returns>Position</returns>
        Vector3 GetPosition(float time);

        /// <summary>
        /// Gets the approximate length
        /// </summary>
        /// <param name="segmentCount">Segment count</param>
        /// <returns></returns>
        float GetApproximateLength(uint segmentCount);

        /// <summary>
        /// Gets approximate segments
        /// </summary>
        /// <param name="approximateSegments">Approximate segments</param>
        /// <param name="offset">Offset</param>
        /// <param name="segmentCount">Segment count</param>
        /// <returns></returns>
        IApproximateBezierCurveKeySegment[] GetApproximateSegments(IApproximateBezierCurveKeySegment[] approximateSegments, uint offset, uint segmentCount);
    }
}
