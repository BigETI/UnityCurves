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
        public static float GetApproximateLength(IReadOnlyCollection<BezierCurveKeyData> keys, uint keySegmentCount)
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
        /// <returns></returns>
        public static float GetApproximateLength(IReadOnlyList<IApproximateBezierCurveKeySegment> segments)
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
        /// Applies Bézier curve key data properties
        /// </summary>
        /// <param name="keys">Keys</param>
        /// <param name="index">Index</param>
        /// <param name="startPosition">Start position</param>
        /// <param name="endPosition">End position</param>
        /// <param name="startTangent">Start tangent</param>
        /// <param name="endTangent">End tangent</param>
        /// <param name="isEndPositionConnected">Is end position connected</param>
        /// <param name="isEndTangentConnected">Is end tangent connected</param>
        public static void ApplyBezierCurveKeyDataProperties(IList<BezierCurveKeyData> keys, int index, Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent, bool isEndPositionConnected, bool isEndTangentConnected)
        {
            if (keys == null)
            {
                throw new ArgumentNullException(nameof(keys));
            }
            if ((index < 0) || (index >= keys.Count))
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, "Index must be equal or greater than zero and smaller than the number of keys.");
            }
            BezierCurveKeyData chargine_eye_path_key = keys[index];
            int next_selected_index = index + 1;
            int previous_selected_index = index - 1;
            bool is_previous_end_position_connected = index == 0;
            if ((isEndPositionConnected || isEndTangentConnected || chargine_eye_path_key.EndTangent != endTangent) && (next_selected_index < keys.Count))
            {
                BezierCurveKeyData key = keys[next_selected_index];
                keys[next_selected_index] = new BezierCurveKeyData(isEndPositionConnected ? Vector3.zero : key.StartPosition, key.EndPosition, isEndTangentConnected ? -(endTangent - endPosition) : key.StartTangent, key.EndTangent, key.IsEndPositionConnected, key.IsEndTangentConnected);
            }
            if ((previous_selected_index >= 0) && ((chargine_eye_path_key.StartPosition != startPosition) || (chargine_eye_path_key.StartTangent != startTangent)))
            {
                BezierCurveKeyData key = keys[previous_selected_index];
                is_previous_end_position_connected = key.IsEndPositionConnected;
                if (is_previous_end_position_connected || key.IsEndTangentConnected)
                {
                    keys[previous_selected_index] = new BezierCurveKeyData(key.StartPosition, is_previous_end_position_connected ? (key.EndPosition + startPosition) : key.EndPosition, key.StartTangent, key.IsEndTangentConnected ? (key.EndPosition - startTangent) : key.EndTangent, is_previous_end_position_connected, key.IsEndTangentConnected);
                }
            }
            keys[index] = new BezierCurveKeyData(is_previous_end_position_connected ? Vector3.zero : startPosition, endPosition, startTangent, endTangent, isEndPositionConnected, isEndTangentConnected);
        }

        /// <summary>
        /// Gets the approximate Bézier curve object state
        /// </summary>
        /// <param name="approximateBezierCurveKeySegments">Approximate Bézier curve key segments</param>
        /// <param name="progress">Progress</param>
        /// <param name="progressWrapMode">Progress wrap mode</param>
        /// <param name="isInterpolatingForwardVector">Is interpolating forward vector</param>
        /// <returns>Approximate Bézier curve object state</returns>
        public static IApproximateBezierCurveObjectState GetApproximateBezierCurveObjectState(IApproximateBezierCurveKeySegments approximateBezierCurveKeySegments, float progress, EProgressWrapMode progressWrapMode, bool isInterpolatingForwardVector)
        {
            Vector3 position = Vector3.zero;
            Vector3 forward = Vector3.forward;
            float remaining_progress;
            switch (progressWrapMode)
            {
                case EProgressWrapMode.Clamp:
                    remaining_progress = Mathf.Clamp(progress, 0.0f, approximateBezierCurveKeySegments.Length);
                    break;
                case EProgressWrapMode.Repeat:
                    remaining_progress = Mathf.Repeat(progress, approximateBezierCurveKeySegments.Length);
                    break;
                case EProgressWrapMode.Reverse:
                    remaining_progress = Mathf.Repeat(progress, approximateBezierCurveKeySegments.Length * 2.0f);
                    if (remaining_progress > approximateBezierCurveKeySegments.Length)
                    {
                        remaining_progress = approximateBezierCurveKeySegments.Length - (remaining_progress - approximateBezierCurveKeySegments.Length);
                    }
                    break;
                default:
                    throw new NotImplementedException($"progress wrap mode for \"{ progressWrapMode }\" getting approximate Bézier curve object state has not been implemented yet.");
            }
            for (int baked_charging_eye_path_key_segment_index = 0; baked_charging_eye_path_key_segment_index < approximateBezierCurveKeySegments.Segments.Count; baked_charging_eye_path_key_segment_index++)
            {
                IApproximateBezierCurveKeySegment baked_charging_eye_path_key_segment = approximateBezierCurveKeySegments.Segments[baked_charging_eye_path_key_segment_index];
                if (remaining_progress > baked_charging_eye_path_key_segment.Length)
                {
                    position += baked_charging_eye_path_key_segment.Delta;
                    forward = baked_charging_eye_path_key_segment.Forward;
                    remaining_progress -= baked_charging_eye_path_key_segment.Length;
                }
                else
                {
                    position += baked_charging_eye_path_key_segment.Forward * remaining_progress;
                    forward = baked_charging_eye_path_key_segment.Forward;
                    if (isInterpolatingForwardVector)
                    {
                        float double_remaining_progress = remaining_progress * 2.0f;
                        if (double_remaining_progress < baked_charging_eye_path_key_segment.Length)
                        {
                            int previous_baked_charging_eye_path_key_segment_index = baked_charging_eye_path_key_segment_index - 1;
                            if (previous_baked_charging_eye_path_key_segment_index >= 0)
                            {
                                forward = Vector3.Slerp(approximateBezierCurveKeySegments.Segments[previous_baked_charging_eye_path_key_segment_index].Forward, forward, (remaining_progress / baked_charging_eye_path_key_segment.Length) + 0.5f);
                            }
                        }
                        else
                        {
                            int next_baked_charging_eye_path_key_segment_index = baked_charging_eye_path_key_segment_index + 1;
                            if (next_baked_charging_eye_path_key_segment_index < approximateBezierCurveKeySegments.Segments.Count)
                            {
                                forward = Vector3.Slerp(forward, approximateBezierCurveKeySegments.Segments[next_baked_charging_eye_path_key_segment_index].Forward, (remaining_progress / baked_charging_eye_path_key_segment.Length) - 0.5f);
                            }
                        }
                    }
                    break;
                }
            }
            return new ApproximateBezierCurveObjectState(position, forward);
        }
    }
}
