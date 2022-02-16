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
            if (selected_game_object && selected_game_object.TryGetComponent(out CurvesControllerScript curves_controller))
            {
                CurvesControllerEditorScript current_curves_controller_editor = CurvesControllerEditorScript.CurrentCurvesControllerEditor;
                if (current_curves_controller_editor)
                {
                    List<BezierCurveKeyData> keys = curves_controller.TargetPath.Keys;
                    int selected_index = current_curves_controller_editor.SelectedIndex;
                    if ((selected_index >= 0) && (selected_index < keys.Count))
                    {
                        BezierCurveKeyData chargine_eye_path_key = keys[selected_index];
                        IsShowingAbsolutePositions = EditorGUILayout.Toggle("Show absolute positions", IsShowingAbsolutePositions);
                        Vector3 offset = IsShowingAbsolutePositions ? GetAbsoluteOffset(curves_controller.TargetPath.Keys, selected_index) : Vector3.zero;
                        Vector3 new_start_position = EditorGUILayout.Vector3Field("Start position (relative)", chargine_eye_path_key.StartPosition + offset) - offset;
                        Vector3 new_end_position = EditorGUILayout.Vector3Field("End position (relative)", chargine_eye_path_key.EndPosition + offset) - offset;
                        Vector3 new_start_tangent = EditorGUILayout.Vector3Field("Start tangent (relative)", chargine_eye_path_key.StartTangent + offset) - offset;
                        Vector3 new_end_tangent = EditorGUILayout.Vector3Field("End tangent (relative)", chargine_eye_path_key.EndTangent + offset) - offset;
                        bool new_is_end_position_connected = EditorGUILayout.Toggle("Is end position connected", chargine_eye_path_key.IsEndPositionConnected);
                        bool new_is_end_tangent_connected = EditorGUILayout.Toggle("Is end tangent connected", chargine_eye_path_key.IsEndTangentConnected);
                        if
                        (
                            (chargine_eye_path_key.StartPosition != new_start_position) ||
                            (chargine_eye_path_key.EndPosition != new_end_position) ||
                            (chargine_eye_path_key.StartTangent != new_start_tangent) ||
                            (chargine_eye_path_key.EndTangent != new_end_tangent) ||
                            (chargine_eye_path_key.IsEndPositionConnected != new_is_end_position_connected) ||
                            (chargine_eye_path_key.IsEndTangentConnected != new_is_end_tangent_connected)
                        )
                        {
                            if (selected_index == 0)
                            {
                                Undo.RecordObjects(new Object[] { curves_controller.transform, curves_controller }, "Modify curve properties");
                                curves_controller.transform.localPosition += new_start_position;
                                new_start_position = Vector3.zero;
                            }
                            else
                            {
                                Undo.RecordObject(curves_controller, "Modify curve properties");
                            }
                            BezierCurveUtilities.ApplyBezierCurveKeyDataProperties(keys, selected_index, new_start_position, new_end_position, new_start_tangent, new_end_tangent, new_is_end_position_connected, new_is_end_tangent_connected);
                            EditorUtility.SetDirty(curves_controller);
                            EditorApplication.QueuePlayerLoopUpdate();
                        }
                        if (GUILayout.Button("Insert key before"))
                        {
                            Undo.RecordObject(curves_controller, $"Insert curve at index { selected_index }");
                            keys.Insert(selected_index, new BezierCurveKeyData(chargine_eye_path_key.StartPosition, chargine_eye_path_key.EndPosition, chargine_eye_path_key.StartTangent, chargine_eye_path_key.EndTangent, false, false));
                            BezierCurveUtilities.ApplyBezierCurveKeyDataProperties(keys, selected_index, chargine_eye_path_key.StartPosition, chargine_eye_path_key.EndPosition, chargine_eye_path_key.StartTangent, chargine_eye_path_key.EndTangent, chargine_eye_path_key.IsEndPositionConnected, chargine_eye_path_key.IsEndTangentConnected);
                            EditorUtility.SetDirty(curves_controller);
                            EditorApplication.QueuePlayerLoopUpdate();
                        }
                        if (GUILayout.Button("Insert key after"))
                        {
                            int new_selected_index = selected_index + 1;
                            Undo.RecordObject(curves_controller, $"Insert curve at index { new_selected_index }");
                            keys.Insert(new_selected_index, new BezierCurveKeyData(chargine_eye_path_key.StartPosition, chargine_eye_path_key.EndPosition, chargine_eye_path_key.StartTangent, chargine_eye_path_key.EndTangent, false, false));
                            BezierCurveUtilities.ApplyBezierCurveKeyDataProperties(keys, selected_index, chargine_eye_path_key.StartPosition, chargine_eye_path_key.EndPosition, chargine_eye_path_key.StartTangent, chargine_eye_path_key.EndTangent, chargine_eye_path_key.IsEndPositionConnected, chargine_eye_path_key.IsEndTangentConnected);
                            BezierCurveUtilities.ApplyBezierCurveKeyDataProperties(keys, new_selected_index, chargine_eye_path_key.StartPosition, chargine_eye_path_key.EndPosition, chargine_eye_path_key.StartTangent, chargine_eye_path_key.EndTangent, chargine_eye_path_key.IsEndPositionConnected, chargine_eye_path_key.IsEndTangentConnected);
                            current_curves_controller_editor.SelectedIndex = new_selected_index;
                            EditorUtility.SetDirty(curves_controller);
                            EditorApplication.QueuePlayerLoopUpdate();
                        }
                        if (GUILayout.Button("Remove key"))
                        {
                            int previous_selected_index = selected_index - 1;
                            Undo.RecordObject(curves_controller, $"Remove curve at index { selected_index }");
                            keys.RemoveAt(selected_index);
                            if (previous_selected_index >= 0)
                            {
                                BezierCurveKeyData key = keys[previous_selected_index];
                                BezierCurveUtilities.ApplyBezierCurveKeyDataProperties(keys, previous_selected_index, key.StartPosition, key.EndPosition, key.StartTangent, key.EndTangent, key.IsEndPositionConnected, key.IsEndTangentConnected);
                            }
                            EditorUtility.SetDirty(curves_controller);
                            EditorApplication.QueuePlayerLoopUpdate();
                        }
                    }
                }
            }
            GUILayout.EndScrollView();
        }
    }
}
