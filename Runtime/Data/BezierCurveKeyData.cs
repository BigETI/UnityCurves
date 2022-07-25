using System;
using UnityEngine;

/// <summary>
/// Unity Curves data namespace
/// </summary>
namespace UnityCurves.Data
{
    /// <summary>
    /// A class that describes bezier curve key data
    /// </summary>
    [Serializable]
    public struct BezierCurveKeyData : IBezierCurveKeyData
    {
        /// <summary>
        /// Start position
        /// </summary>
        [SerializeField]
        private Vector3 startPosition;

        /// <summary>
        /// End position
        /// </summary>
        [SerializeField]
        private Vector3 endPosition;

        /// <summary>
        /// Start up vector angle in degrees
        /// </summary>
        [SerializeField]
        private float startUpVectorAngle;

        /// <summary>
        /// End up vector angle in degrees
        /// </summary>
        [SerializeField]
        private float endUpVectorAngle;

        /// <summary>
        /// Start tangent
        /// </summary>
        [SerializeField]
        private Vector3 startTangent;

        /// <summary>
        /// End tangent
        /// </summary>
        [SerializeField]
        private Vector3 endTangent;

        /// <summary>
        /// Is end position connected
        /// </summary>
        [SerializeField]
        private bool isEndPositionConnected;

        /// <summary>
        /// Is end up vector angle connected
        /// </summary>
        [SerializeField]
        private bool isEndUpVectorAngleConnected;

        /// <summary>
        /// Is end tangent connected
        /// </summary>
        [SerializeField]
        private bool isEndTangentConnected;

        /// <summary>
        /// Start position
        /// </summary>
        public Vector3 StartPosition
        {
            get => startPosition;
            set => startPosition = value;
        }

        /// <summary>
        /// End position
        /// </summary>
        public Vector3 EndPosition
        {
            get => endPosition;
            set => endPosition = value;
        }

        /// <summary>
        /// Start up vector angle in degrees
        /// </summary>
        public float StartUpVectorAngle
        {
            get => startUpVectorAngle;
            set => startUpVectorAngle = value;
        }

        /// <summary>
        /// End up vector angle in degrees
        /// </summary>
        public float EndUpVectorAngle
        {
            get => endUpVectorAngle;
            set => endUpVectorAngle = value;
        }

        /// <summary>
        /// Start tangent
        /// </summary>
        public Vector3 StartTangent
        {
            get => startTangent;
            set => startTangent = value;
        }

        /// <summary>
        /// End tangent
        /// </summary>
        public Vector3 EndTangent
        {
            get => endTangent;
            set => endTangent = value;
        }

        /// <summary>
        /// Is end position connected
        /// </summary>
        public bool IsEndPositionConnected
        {
            get => isEndPositionConnected;
            set => isEndPositionConnected = value;
        }
        
        /// <summary>
        /// Is end up vector angle connected
        /// </summary>
        public bool IsEndUpVectorAngleConnected
        {
            get => isEndUpVectorAngleConnected;
            set => isEndUpVectorAngleConnected = value;
        }

        /// <summary>
        /// Is end tangent connected
        /// </summary>
        public bool IsEndTangentConnected
        {
            get => isEndTangentConnected;
            set => isEndTangentConnected = value;
        }

        /// <summary>
        /// Constructs new bezier curve key data
        /// </summary>
        /// <param name="startPosition"></param>
        /// <param name="endPosition"></param>
        /// <param name="startUpVectorAngle">Start up vector angle in degrees</param>
        /// <param name="endUpVectorAngle">End up vector angle in degrees</param>
        /// <param name="startTangent"></param>
        /// <param name="endTangent"></param>
        /// <param name="isEndPositionConnected"></param>
        /// <param name="isEndUpVectorAngleConnected">Is end up vector angle connected</param>
        /// <param name="isEndTangentConnected"></param>
        public BezierCurveKeyData(Vector3 startPosition, Vector3 endPosition, float startUpVectorAngle, float endUpVectorAngle, Vector3 startTangent, Vector3 endTangent, bool isEndPositionConnected, bool isEndUpVectorAngleConnected, bool isEndTangentConnected)
        {
            this.startPosition = startPosition;
            this.endPosition = endPosition;
            this.startUpVectorAngle = startUpVectorAngle;
            this.endUpVectorAngle = endUpVectorAngle;
            this.startTangent = startTangent;
            this.endTangent = endTangent;
            this.isEndPositionConnected = isEndPositionConnected;
            this.isEndUpVectorAngleConnected = isEndUpVectorAngleConnected;
            this.isEndTangentConnected = isEndTangentConnected;
        }

        /// <summary>
        /// Gets position at the specified time
        /// </summary>
        /// <param name="time">Time from 0 to 1</param>
        /// <returns>Position</returns>
        public Vector3 GetPosition(float time)
        {
            float clamped_time = Mathf.Clamp01(time);
            float remaining_time = 1f - clamped_time;
            float time_squared = clamped_time * clamped_time;
            float remaining_time_squared = remaining_time * remaining_time;
            float remaining_time_cubed = remaining_time_squared * remaining_time;
            float time_cubed = time_squared * clamped_time;
            return (remaining_time_cubed * startPosition) + (3.0f * remaining_time_squared * clamped_time * startTangent) + (3.0f * remaining_time * time_squared * endTangent) + (time_cubed * endPosition);
        }

        /// <summary>
        /// Gets the approximate length
        /// </summary>
        /// <param name="segmentCount">Segment count</param>
        /// <returns></returns>
        public float GetApproximateLength(uint segmentCount)
        {
            if (segmentCount == 0U)
            {
                throw new ArgumentException("Segment count must be greater than zero.", nameof(segmentCount));
            }
            float ret = 0.0f;
            Vector3 start_position = startPosition;
            for (uint index = 0; index != segmentCount; index++)
            {
                float time = (index + 1U) / (float)segmentCount;
                Vector3 end_position = GetPosition(time);
                ret += Vector3.Distance(start_position, end_position);
                start_position = end_position;
            }
            return ret;
        }

        /// <summary>
        /// Gets approximate segments
        /// </summary>
        /// <param name="approximateSegments">Approximate segments</param>
        /// <param name="offset">Offset</param>
        /// <param name="segmentCount">Segment count</param>
        /// <returns></returns>
        public IApproximateBezierCurveKeySegment[] GetApproximateSegments(IApproximateBezierCurveKeySegment[] approximateSegments, uint offset, uint segmentCount)
        {
            if (approximateSegments == null)
            {
                throw new ArgumentNullException(nameof(approximateSegments));
            }
            if (offset >= approximateSegments.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), offset, "Offset can't be equal or greater than approximate segment length count");
            }
            if (segmentCount == 0U)
            {
                throw new ArgumentOutOfRangeException(nameof(segmentCount), "Segment count can't be zero.");
            }
            if ((offset + segmentCount) > approximateSegments.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(segmentCount), "Segment count is too big.");
            }
            Vector3 start_position = startPosition;
            for (uint index = 0U; index != segmentCount; index++)
            {
                float time = (index + 1U) / (float)segmentCount;
                Vector3 end_position = GetPosition(time);
                Vector3 delta = end_position - start_position;
                float length = delta.magnitude;
                approximateSegments[offset + index] = new ApproximateBezierCurveKeySegment(length, delta / length, Mathf.Lerp(startUpVectorAngle, endUpVectorAngle, time), delta);
                start_position = end_position;
            }
            return approximateSegments;
        }
    }
}
