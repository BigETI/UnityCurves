using System;
using System.Collections.Generic;
using UnityCurves.Data;
using UnityEngine;

/// <summary>
/// Unity Curves namespace
/// </summary>
namespace UnityCurves
{
    /// <summary>
    /// Bézier curve utilities
    /// </summary>
    public static class BezierCurveUtilities
    {
        /// <summary>
        /// Gets the approximate length of a Bézier curve
        /// </summary>
        /// <param name="keys">Keys</param>
        /// <param name="keySegmentCount">Key segment count</param>
        /// <returns></returns>
        public static float GetApproximateLength(IEnumerable<BezierCurveKeyData> keys, uint keySegmentCount)
        {
            if (keys == null)
            {
                throw new ArgumentNullException(nameof(keys));
            }
            float ret = 0.0f;
            foreach (BezierCurveKeyData key in keys)
            {
                ret += key.GetApproximateLength(keySegmentCount);
            }
            return ret;
        }

        /// <summary>
        /// Gets the approximate length of a Bézier curve
        /// </summary>
        /// <param name="keys">Keys</param>
        /// <param name="keySegmentCount">Key segment count</param>
        /// <returns>Approximate length of a Bézier curve</returns>
        public static float GetApproximateLength(IEnumerable<IApproximateBezierCurveKeySegment> segments)
        {
            if (segments == null)
            {
                throw new ArgumentNullException(nameof(segments));
            }
            float ret = 0.0f;
            foreach (IApproximateBezierCurveKeySegment segment in segments)
            {
                ret += segment.Length;
            }
            return ret;
        }

        /// <summary>
        /// Gets the closest point to a Bézier curve
        /// </summary>
        /// <param name="segments">Segments</param>
        /// <param name="position">Position</param>
        /// <returns>Closest point to a Bézier curve</returns>
        /// <exception cref="ArgumentNullException">When "segments" is "null"</exception>
        public static IApproximateBezierCurvePoint GetClosestPointToBezierCurve(IEnumerable<IApproximateBezierCurveKeySegment> segments, Vector3 position)
        {
            if (segments == null)
            {
                throw new ArgumentNullException(nameof(segments));
            }
            (float DistanceSquaredTo, Vector3 Position, Vector3 Forward, float UpVectorAngle, float Progress) primary_segment = (float.PositiveInfinity, Vector3.positiveInfinity, Vector3.up, 0.0f, 0.0f);
            (float DistanceSquaredTo, Vector3 Position, Vector3 Forward, float UpVectorAngle, float Progress) secondary_segment = (float.PositiveInfinity, Vector3.positiveInfinity, Vector3.up, 0.0f, 0.0f);
            (float DistanceSquaredTo, Vector3 Position, Vector3 Forward, float UpVectorAngle, float Progress) previous_segment = (float.PositiveInfinity, Vector3.positiveInfinity, Vector3.up, 0.0f, 0.0f);
            bool is_closer_distance_found = false;
            bool is_right_segment_closer = false;
            Vector3 current_position = Vector3.zero;
            float current_progress = 0.0f;
            Vector3 closest_position = Vector3.positiveInfinity;
            Vector3 closest_forward = Vector3.forward;
            float closest_up_vector_angle = 0.0f;
            float closest_progress = 0.0f;
            foreach (IApproximateBezierCurveKeySegment segment in segments)
            {
                current_position += segment.Delta;
                current_progress += segment.Length;
                float to_segment_distance_squared = (current_position - position).sqrMagnitude;
                if (primary_segment.DistanceSquaredTo > to_segment_distance_squared)
                {
                    primary_segment = (to_segment_distance_squared, current_position, segment.Forward, segment.UpVectorAngle, current_progress);
                    secondary_segment = previous_segment;
                    is_closer_distance_found = true;
                }
                else if (is_closer_distance_found)
                {
                    is_closer_distance_found = false;
                    is_right_segment_closer = to_segment_distance_squared < secondary_segment.DistanceSquaredTo;
                    if (is_right_segment_closer)
                    {
                        secondary_segment = (to_segment_distance_squared, current_position, segment.Forward, segment.UpVectorAngle, current_progress);
                    }
                }
                previous_segment = (to_segment_distance_squared, current_position, segment.Forward, segment.UpVectorAngle, current_progress);
            }
            if (primary_segment.DistanceSquaredTo < float.PositiveInfinity)
            {
                if (secondary_segment.DistanceSquaredTo < float.PositiveInfinity)
                {
                    (float DistanceSquaredTo, Vector3 Position, Vector3 Forward, float UpVectorAngle, float Progress) left_segment = is_right_segment_closer ? primary_segment : secondary_segment;
                    (float DistanceSquaredTo, Vector3 Position, Vector3 Forward, float UpVectorAngle, float Progress) right_segment = is_right_segment_closer ? secondary_segment : primary_segment;
                    Vector3 delta = right_segment.Position - left_segment.Position;
                    float delta_magnitude_squared = delta.sqrMagnitude;
                    if (delta_magnitude_squared > float.Epsilon)
                    {
                        float delta_magnitude = Mathf.Sqrt(delta_magnitude_squared);
                        Vector3 direction = delta / delta_magnitude;
                        Vector3 left_segment_to_position_vector = position - left_segment.Position;
                        float clamped_projection_distance = Mathf.Clamp(Vector3.Dot(left_segment_to_position_vector, direction), 0.0f, delta_magnitude);
                        float normalized_clamped_projection_distance = clamped_projection_distance / delta_magnitude;
                        closest_position = left_segment.Position + (clamped_projection_distance * direction);
                        closest_forward = Vector3.Slerp(left_segment.Forward, right_segment.Forward, normalized_clamped_projection_distance);
                        closest_up_vector_angle = Mathf.Lerp(left_segment.UpVectorAngle, right_segment.UpVectorAngle, normalized_clamped_projection_distance);
                        closest_progress = Mathf.Lerp(left_segment.Progress, right_segment.Progress, normalized_clamped_projection_distance);
                    }
                    else
                    {
                        closest_position = left_segment.Position;
                        closest_forward = left_segment.Forward;
                        closest_up_vector_angle = left_segment.UpVectorAngle;
                        closest_progress = left_segment.Progress;
                    }
                }
                else
                {
                    closest_position = primary_segment.Position;
                    closest_forward = primary_segment.Forward;
                    closest_up_vector_angle = primary_segment.UpVectorAngle;
                    closest_progress = primary_segment.Progress;
                }
            }
            return new ApproximateBezierCurvePoint(closest_position, closest_forward, Quaternion.AngleAxis(closest_up_vector_angle, closest_forward) * Vector3.up, closest_progress);
        }

        /// <summary>
        /// Applies Bézier curve key data properties
        /// </summary>
        /// <param name="keys">Keys</param>
        /// <param name="index">Index</param>
        /// <param name="startPosition">Start position</param>
        /// <param name="endPosition">End position</param>
        /// <param name="startUpVectorAngle">Start up vector angle</param>
        /// <param name="endUpVectorAngle">End up vector angle</param>
        /// <param name="startTangent">Start tangent</param>
        /// <param name="endTangent">End tangent</param>
        /// <param name="isEndPositionConnected">Is end position connected</param>
        /// <param name="isEndUpVectorAngleConnected">Is end up vector angle connected</param>
        /// <param name="isEndTangentConnected">Is end tangent connected</param>
        public static void ApplyBezierCurveKeyDataProperties(IList<BezierCurveKeyData> keys, int index, Vector3 startPosition, Vector3 endPosition, float startUpVectorAngle, float endUpVectorAngle, Vector3 startTangent, Vector3 endTangent, bool isEndPositionConnected, bool isEndUpVectorAngleConnected, bool isEndTangentConnected)
        {
            if (keys == null)
            {
                throw new ArgumentNullException(nameof(keys));
            }
            if ((index < 0) || (index >= keys.Count))
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, "Index must be equal or greater than zero and smaller than the number of keys.");
            }
            BezierCurveKeyData bezier_curve_key_data = keys[index];
            int next_selected_index = index + 1;
            int previous_selected_index = index - 1;
            bool is_previous_end_position_connected = index == 0;
            if ((isEndPositionConnected || isEndUpVectorAngleConnected || isEndTangentConnected || (bezier_curve_key_data.EndTangent != endTangent)) && (next_selected_index < keys.Count))
            {
                BezierCurveKeyData next_bezier_curve_key_data = keys[next_selected_index];
                keys[next_selected_index] = new BezierCurveKeyData(isEndPositionConnected ? Vector3.zero : next_bezier_curve_key_data.StartPosition, next_bezier_curve_key_data.EndPosition, isEndUpVectorAngleConnected ? endUpVectorAngle : next_bezier_curve_key_data.StartUpVectorAngle, next_bezier_curve_key_data.EndUpVectorAngle, isEndTangentConnected ? -(endTangent - endPosition) : next_bezier_curve_key_data.StartTangent, next_bezier_curve_key_data.EndTangent, next_bezier_curve_key_data.IsEndPositionConnected, next_bezier_curve_key_data.IsEndUpVectorAngleConnected, next_bezier_curve_key_data.IsEndTangentConnected);
            }
            if ((previous_selected_index >= 0) && ((bezier_curve_key_data.StartPosition != startPosition) || (bezier_curve_key_data.StartUpVectorAngle != startUpVectorAngle) || (bezier_curve_key_data.StartTangent != startTangent)))
            {
                BezierCurveKeyData previous_bezier_curve_key_data = keys[previous_selected_index];
                is_previous_end_position_connected = previous_bezier_curve_key_data.IsEndPositionConnected;
                if (is_previous_end_position_connected || previous_bezier_curve_key_data.IsEndUpVectorAngleConnected || previous_bezier_curve_key_data.IsEndTangentConnected)
                {
                    keys[previous_selected_index] = new BezierCurveKeyData(previous_bezier_curve_key_data.StartPosition, is_previous_end_position_connected ? (previous_bezier_curve_key_data.EndPosition + startPosition) : previous_bezier_curve_key_data.EndPosition, previous_bezier_curve_key_data.StartUpVectorAngle, previous_bezier_curve_key_data.IsEndUpVectorAngleConnected ? startUpVectorAngle : previous_bezier_curve_key_data.EndUpVectorAngle, previous_bezier_curve_key_data.StartTangent, previous_bezier_curve_key_data.IsEndTangentConnected ? (previous_bezier_curve_key_data.EndPosition - startTangent) : previous_bezier_curve_key_data.EndTangent, is_previous_end_position_connected, previous_bezier_curve_key_data.IsEndUpVectorAngleConnected, previous_bezier_curve_key_data.IsEndTangentConnected);
                }
            }
            keys[index] = new BezierCurveKeyData(is_previous_end_position_connected ? Vector3.zero : startPosition, endPosition, startUpVectorAngle, endUpVectorAngle, startTangent, endTangent, isEndPositionConnected, isEndUpVectorAngleConnected, isEndTangentConnected);
        }

        /// <summary>
        /// Gets the approximate Bézier curve object state
        /// </summary>
        /// <param name="approximateBezierCurveKeySegments">Approximate Bézier curve key segments</param>
        /// <param name="progress">Progress</param>
        /// <param name="progressWrapMode">Progress wrap mode</param>
        /// <param name="isInterpolatingForwardVector">Is interpolating forward vector</param>
        /// <returns>Approximate Bézier curve object state</returns>
        public static IApproximateBezierCurvePoint GetApproximateBezierCurveObjectState(IApproximateBezierCurveKeySegments approximateBezierCurveKeySegments, float progress, EProgressWrapMode progressWrapMode, bool isInterpolatingForwardVector)
        {
            Vector3 position = Vector3.zero;
            Vector3 forward = Vector3.forward;
            Vector3 up = Vector3.up;
            float clamped_progress;
            float remaining_progress;
            switch (progressWrapMode)
            {
                case EProgressWrapMode.Clamp:
                    clamped_progress = Mathf.Clamp(progress, 0.0f, approximateBezierCurveKeySegments.Length);
                    remaining_progress = clamped_progress;
                    break;
                case EProgressWrapMode.Repeat:
                    clamped_progress = Mathf.Repeat(progress, approximateBezierCurveKeySegments.Length);
                    remaining_progress = clamped_progress;
                    break;
                case EProgressWrapMode.Reverse:
                    clamped_progress = Mathf.Repeat(progress, approximateBezierCurveKeySegments.Length * 2.0f);
                    if (clamped_progress > approximateBezierCurveKeySegments.Length)
                    {
                        clamped_progress = approximateBezierCurveKeySegments.Length - (clamped_progress - approximateBezierCurveKeySegments.Length);
                    }
                    remaining_progress = clamped_progress;
                    break;
                default:
                    throw new NotImplementedException($"progress wrap mode for \"{ progressWrapMode }\" getting approximate Bézier curve object state has not been implemented yet.");
            }
            for (int approximate_bezier_curve_key_segment_index = 0; approximate_bezier_curve_key_segment_index < approximateBezierCurveKeySegments.Segments.Count; approximate_bezier_curve_key_segment_index++)
            {
                IApproximateBezierCurveKeySegment approximate_bezier_curve_key_segment = approximateBezierCurveKeySegments.Segments[approximate_bezier_curve_key_segment_index];
                if (remaining_progress > approximate_bezier_curve_key_segment.Length)
                {
                    position += approximate_bezier_curve_key_segment.Delta;
                    forward = approximate_bezier_curve_key_segment.Forward;
                    up = Quaternion.AngleAxis(approximate_bezier_curve_key_segment.UpVectorAngle, forward) * Vector3.up;
                    remaining_progress -= approximate_bezier_curve_key_segment.Length;
                }
                else
                {
                    position += approximate_bezier_curve_key_segment.Forward * remaining_progress;
                    forward = approximate_bezier_curve_key_segment.Forward;
                    up = Quaternion.AngleAxis(approximate_bezier_curve_key_segment.UpVectorAngle, forward) * Vector3.up;
                    if (isInterpolatingForwardVector)
                    {
                        float double_remaining_progress = remaining_progress * 2.0f;
                        if (double_remaining_progress < approximate_bezier_curve_key_segment.Length)
                        {
                            int previous_approximate_bezier_curve_key_segment_index = approximate_bezier_curve_key_segment_index - 1;
                            if (previous_approximate_bezier_curve_key_segment_index >= 0)
                            {
                                IApproximateBezierCurveKeySegment previous_approximate_bezier_curve_key_segment = approximateBezierCurveKeySegments.Segments[previous_approximate_bezier_curve_key_segment_index];
                                float time = (remaining_progress / approximate_bezier_curve_key_segment.Length) + 0.5f;
                                forward = Vector3.Slerp(previous_approximate_bezier_curve_key_segment.Forward, forward, time);
                                up = Quaternion.AngleAxis(Mathf.Lerp(previous_approximate_bezier_curve_key_segment.UpVectorAngle, approximate_bezier_curve_key_segment.UpVectorAngle, time), forward) * Vector3.up;
                            }
                        }
                        else
                        {
                            int next_approximate_bezier_curve_key_segment_index = approximate_bezier_curve_key_segment_index + 1;
                            if (next_approximate_bezier_curve_key_segment_index < approximateBezierCurveKeySegments.Segments.Count)
                            {
                                IApproximateBezierCurveKeySegment next_approximate_bezier_curve_key_segment = approximateBezierCurveKeySegments.Segments[next_approximate_bezier_curve_key_segment_index];
                                float time = (remaining_progress / approximate_bezier_curve_key_segment.Length) - 0.5f;
                                forward = Vector3.Slerp(forward, next_approximate_bezier_curve_key_segment.Forward, time);
                                up = Quaternion.AngleAxis(Mathf.Lerp(approximate_bezier_curve_key_segment.UpVectorAngle, next_approximate_bezier_curve_key_segment.UpVectorAngle, time), forward) * Vector3.up;
                            }
                        }
                    }
                    break;
                }
            }
            return new ApproximateBezierCurvePoint(position, forward, up, clamped_progress);
        }
    }
}
