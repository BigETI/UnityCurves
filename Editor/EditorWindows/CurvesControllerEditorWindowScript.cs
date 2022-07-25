using System.Collections.Generic;
using UnityCurves;
using UnityCurves.Controllers;
using UnityCurves.Data;
using UnityCurvesEditor.EditorScripts;
using UnityEditor;
using UnityEngine;
using UnityPatternsEditor.EditorWindows;

/// <summary>
/// Unity Curves editor editor windows namespace
/// </summary>
namespace UnityCurvesEditor.EditorWindows
{
    /// <summary>
    /// A class that describes a curves controller editor window script
    /// </summary>
    public class CurvesControllerEditorWindowScript : AEditorWindow, ICurvesControllerEditorWindow
    {
        /// <summary>
        /// Scroll position
        /// </summary>
        public Vector2 ScrollPosition { get; set; }

        /// <summary>
        /// Is showing absolute positions
        /// </summary>
        public bool IsShowingAbsolutePositions { get; set; }

        /// <summary>
        /// Current window
        /// </summary>
        public static CurvesControllerEditorWindowScript CurrentWindow { get; private set; }

        /// <summary>
        /// Initializes a window
        /// </summary>
        [MenuItem("Window/Curves/Curve editor")]
        private static void InitializeWindow()
        {
            CurvesControllerEditorWindowScript curves_controller_editor_window = GetWindow<CurvesControllerEditorWindowScript>("Curve editor");
            if (curves_controller_editor_window)
            {
                curves_controller_editor_window.Show();
            }
        }

        /// <summary>
        /// Gets the absolute offset
        /// </summary>
        /// <param name="keys">Keys</param>
        /// <param name="selectedIndex">Selected index</param>
        /// <returns>Absolute offset</returns>
        private Vector3 GetAbsoluteOffset(IReadOnlyList<BezierCurveKeyData> keys, int selectedIndex)
        {
            Vector3 ret = Vector3.zero;
            for (int index = 0; index != selectedIndex; index++)
            {
                ret += keys[index].EndPosition;
            }
            return ret;
        }

        /// <summary>
        /// Gets invoked when script gets enabled
        /// </summary>
        private void OnEnable() => CurrentWindow = this;

        /// <summary>
        /// Gets invoked when script gets disabled
        /// </summary>
        private void OnDisable()
        {
            if (CurrentWindow == this)
            {
                CurrentWindow = null;
            }
        }

        /// <summary>
        /// Gets invoked when a GUI update has been performed
        /// </summary>
        private void OnGUI()
        {
            ScrollPosition = GUILayout.BeginScrollView(ScrollPosition);
            GameObject selected_game_object = Selection.activeGameObject;
            if (selected_game_object && selected_game_object.TryGetComponent(out BaseCurvesControllerScript base_curves_controller))
            {
                CurvesControllerEditorScript current_curves_controller_editor = CurvesControllerEditorScript.CurrentCurvesControllerEditor;
                if (current_curves_controller_editor)
                {
                    List<BezierCurveKeyData> keys = base_curves_controller.Path.Keys;
                    int selected_index = current_curves_controller_editor.SelectedIndex;
                    if ((selected_index >= 0) && (selected_index < keys.Count))
                    {
                        BezierCurveKeyData bezier_curve_key_data = keys[selected_index];
                        IsShowingAbsolutePositions = EditorGUILayout.Toggle("Show absolute positions", IsShowingAbsolutePositions);
                        Vector3 offset = IsShowingAbsolutePositions ? GetAbsoluteOffset(base_curves_controller.Path.Keys, selected_index) : Vector3.zero;
                        Vector3 new_start_position = EditorGUILayout.Vector3Field("Start position (relative)", bezier_curve_key_data.StartPosition + offset) - offset;
                        Vector3 new_end_position = EditorGUILayout.Vector3Field("End position (relative)", bezier_curve_key_data.EndPosition + offset) - offset;
                        float new_start_up_vector_angle = EditorGUILayout.FloatField("Start up vector angle (relative)", bezier_curve_key_data.StartUpVectorAngle);
                        float new_end_up_vector_angle = EditorGUILayout.FloatField("End up vector angle (relative)", bezier_curve_key_data.EndUpVectorAngle);
                        Vector3 new_start_tangent = EditorGUILayout.Vector3Field("Start tangent (relative)", bezier_curve_key_data.StartTangent + offset) - offset;
                        Vector3 new_end_tangent = EditorGUILayout.Vector3Field("End tangent (relative)", bezier_curve_key_data.EndTangent + offset) - offset;
                        bool new_is_end_position_connected = EditorGUILayout.Toggle("Is end position connected", bezier_curve_key_data.IsEndPositionConnected);
                        bool new_is_end_up_vector_angle_connected = EditorGUILayout.Toggle("Is end up vector angle connected", bezier_curve_key_data.IsEndUpVectorAngleConnected);
                        bool new_is_end_tangent_connected = EditorGUILayout.Toggle("Is end tangent connected", bezier_curve_key_data.IsEndTangentConnected);
                        if
                        (
                            (bezier_curve_key_data.StartPosition != new_start_position) ||
                            (bezier_curve_key_data.EndPosition != new_end_position) ||
                            (bezier_curve_key_data.StartUpVectorAngle != new_start_up_vector_angle) ||
                            (bezier_curve_key_data.EndUpVectorAngle != new_end_up_vector_angle) ||
                            (bezier_curve_key_data.StartTangent != new_start_tangent) ||
                            (bezier_curve_key_data.EndTangent != new_end_tangent) ||
                            (bezier_curve_key_data.IsEndPositionConnected != new_is_end_position_connected) ||
                            (bezier_curve_key_data.IsEndTangentConnected != new_is_end_tangent_connected)
                        )
                        {
                            if (selected_index == 0)
                            {
                                Undo.RecordObjects(new Object[] { base_curves_controller.transform, base_curves_controller }, "Modify curve properties");
                                base_curves_controller.transform.localPosition += new_start_position;
                                new_start_position = Vector3.zero;
                            }
                            else
                            {
                                Undo.RecordObject(base_curves_controller, "Modify curve properties");
                            }
                            BezierCurveUtilities.ApplyBezierCurveKeyDataProperties(keys, selected_index, new_start_position, new_end_position, new_start_up_vector_angle, new_end_up_vector_angle, new_start_tangent, new_end_tangent, new_is_end_position_connected, new_is_end_up_vector_angle_connected, new_is_end_tangent_connected);
                            EditorUtility.SetDirty(base_curves_controller);
                            EditorApplication.QueuePlayerLoopUpdate();
                        }
                        if (GUILayout.Button("Insert key before"))
                        {
                            Undo.RecordObject(base_curves_controller, $"Insert curve at index { selected_index }");
                            keys.Insert(selected_index, new BezierCurveKeyData(bezier_curve_key_data.StartPosition, bezier_curve_key_data.EndPosition, bezier_curve_key_data.StartUpVectorAngle, bezier_curve_key_data.EndUpVectorAngle, bezier_curve_key_data.StartTangent, bezier_curve_key_data.EndTangent, false, false, false));
                            BezierCurveUtilities.ApplyBezierCurveKeyDataProperties(keys, selected_index, bezier_curve_key_data.StartPosition, bezier_curve_key_data.EndPosition, bezier_curve_key_data.StartUpVectorAngle, bezier_curve_key_data.EndUpVectorAngle, bezier_curve_key_data.StartTangent, bezier_curve_key_data.EndTangent, bezier_curve_key_data.IsEndPositionConnected, bezier_curve_key_data.IsEndUpVectorAngleConnected, bezier_curve_key_data.IsEndTangentConnected);
                            EditorUtility.SetDirty(base_curves_controller);
                            EditorApplication.QueuePlayerLoopUpdate();
                        }
                        if (GUILayout.Button("Insert key after"))
                        {
                            int new_selected_index = selected_index + 1;
                            Undo.RecordObject(base_curves_controller, $"Insert curve at index { new_selected_index }");
                            keys.Insert(new_selected_index, new BezierCurveKeyData(bezier_curve_key_data.StartPosition, bezier_curve_key_data.EndPosition, bezier_curve_key_data.StartUpVectorAngle, bezier_curve_key_data.EndUpVectorAngle, bezier_curve_key_data.StartTangent, bezier_curve_key_data.EndTangent, false, false, false));
                            BezierCurveUtilities.ApplyBezierCurveKeyDataProperties(keys, selected_index, bezier_curve_key_data.StartPosition, bezier_curve_key_data.EndPosition, bezier_curve_key_data.StartUpVectorAngle, bezier_curve_key_data.EndUpVectorAngle, bezier_curve_key_data.StartTangent, bezier_curve_key_data.EndTangent, bezier_curve_key_data.IsEndPositionConnected, bezier_curve_key_data.IsEndUpVectorAngleConnected, bezier_curve_key_data.IsEndTangentConnected);
                            BezierCurveUtilities.ApplyBezierCurveKeyDataProperties(keys, new_selected_index, bezier_curve_key_data.StartPosition, bezier_curve_key_data.EndPosition, bezier_curve_key_data.StartUpVectorAngle, bezier_curve_key_data.EndUpVectorAngle, bezier_curve_key_data.StartTangent, bezier_curve_key_data.EndTangent, bezier_curve_key_data.IsEndPositionConnected, bezier_curve_key_data.IsEndUpVectorAngleConnected, bezier_curve_key_data.IsEndTangentConnected);
                            current_curves_controller_editor.SelectedIndex = new_selected_index;
                            EditorUtility.SetDirty(base_curves_controller);
                            EditorApplication.QueuePlayerLoopUpdate();
                        }
                        if (GUILayout.Button("Remove key"))
                        {
                            int previous_selected_index = selected_index - 1;
                            Undo.RecordObject(base_curves_controller, $"Remove curve at index { selected_index }");
                            keys.RemoveAt(selected_index);
                            if (previous_selected_index >= 0)
                            {
                                BezierCurveKeyData previous_bezier_curve_key_data = keys[previous_selected_index];
                                BezierCurveUtilities.ApplyBezierCurveKeyDataProperties(keys, previous_selected_index, previous_bezier_curve_key_data.StartPosition, previous_bezier_curve_key_data.EndPosition, previous_bezier_curve_key_data.StartUpVectorAngle, previous_bezier_curve_key_data.EndUpVectorAngle, previous_bezier_curve_key_data.StartTangent, previous_bezier_curve_key_data.EndTangent, previous_bezier_curve_key_data.IsEndPositionConnected, previous_bezier_curve_key_data.IsEndUpVectorAngleConnected, previous_bezier_curve_key_data.IsEndTangentConnected);
                            }
                            EditorUtility.SetDirty(base_curves_controller);
                            EditorApplication.QueuePlayerLoopUpdate();
                        }
                    }
                }
            }
            GUILayout.EndScrollView();
        }
    }
}
