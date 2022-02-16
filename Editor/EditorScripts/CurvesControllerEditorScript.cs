using System.Collections.Generic;
using UnityCurves;
using UnityCurves.Controllers;
using UnityCurves.Data;
using UnityCurvesEditor.EditorWindows;
using UnityEditor;
using UnityEngine;
using UnityPatternsEditor.EditorScripts;

/// <summary>
/// Unity Curves editor editor scripts namespace
/// </summary>
namespace UnityCurvesEditor.EditorScripts
{
    /// <summary>
    /// A class that describes a curves controller editor script
    /// </summary>
    [CustomEditor(typeof(CurvesControllerScript))]
    public class CurvesControllerEditorScript : AEditorScript, ICurvesControllerEditor
    {
        /// <summary>
        /// Label GUI style
        /// </summary>
        private GUIStyle labelGUIStyle;

        /// <summary>
        /// Current curves controller editor
        /// </summary>
        public static CurvesControllerEditorScript CurrentCurvesControllerEditor { get; private set; }

        /// <summary>
        /// Selected index
        /// </summary>
        public int SelectedIndex { get; set; } = -1;

        /// <summary>
        /// Repaints editor window
        /// </summary>
        private void RepaintWindow()
        {
            CurvesControllerEditorWindowScript current_window = CurvesControllerEditorWindowScript.CurrentWindow;
            if (current_window)
            {
                current_window.Repaint();
            }
        }

        /// <summary>
        /// Gets invoked when script gets enabled
        /// </summary>
        private void OnEnable()
        {
            labelGUIStyle = new GUIStyle(GUIStyle.none)
            {
                fontSize = 32,
                richText = true
            };
            CurrentCurvesControllerEditor = this;
        }

        /// <summary>
        /// Gets invoked when script gets disabled
        /// </summary>
        private void OnDisable()
        {
            if (CurrentCurvesControllerEditor == this)
            {
                CurrentCurvesControllerEditor = null;
                SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Gets invoked when scene GUI needs to be updated
        /// </summary>
        private void OnSceneGUI()
        {
            if (target is CurvesControllerScript curves_controller)
            {
                CurvesSettingsScriptableSingleton curves_settings = CurvesSettingsScriptableSingleton.instance;
                if (curves_settings)
                {
                    Matrix4x4 old_matrix = Handles.matrix;
                    Color old_color = Handles.color;
                    Handles.matrix = curves_controller.transform.localToWorldMatrix;
                    Vector3 offset = Vector3.zero;
                    Camera camera = SceneView.currentDrawingSceneView ? SceneView.currentDrawingSceneView.camera : null;
                    Quaternion camera_rotation = camera ? camera.transform.rotation : Quaternion.identity;
                    List<BezierCurveKeyData> keys = curves_controller.TargetPath.Keys;
                    string primary_font_color_code = GetColorCode(curves_settings.PrimaryFontColor);
                    string secondary_font_color_code = GetColorCode(curves_settings.SecondaryFontColor);
                    Handles.CapFunction handle_cap_function = curves_settings.KeyHandleCap switch
                    {
                        EHandleCap.Arrow => Handles.ArrowHandleCap,
                        EHandleCap.Circle => Handles.CircleHandleCap,
                        EHandleCap.Cone => Handles.ConeHandleCap,
                        EHandleCap.Cube => Handles.CubeHandleCap,
                        EHandleCap.Cylinder => Handles.CylinderHandleCap,
                        EHandleCap.Dot => Handles.DotHandleCap,
                        EHandleCap.Rectangle => Handles.RectangleHandleCap,
                        EHandleCap.Sphere => Handles.SphereHandleCap,
                        _ => throw new System.NotImplementedException(),
                    };
                    bool is_handle_cap_2d = curves_settings.KeyHandleCap switch
                    {
                        EHandleCap.Arrow => false,
                        EHandleCap.Circle => true,
                        EHandleCap.Cone => false,
                        EHandleCap.Cube => false,
                        EHandleCap.Cylinder => false,
                        EHandleCap.Dot => true,
                        EHandleCap.Rectangle => true,
                        EHandleCap.Sphere => false,
                        _ => throw new System.NotImplementedException(),
                    };
                    curves_controller.PreviewTargetGizmoType = curves_settings.PreviewTargetGizmoType;
                    curves_controller.PreviewTargetGizmoColor = curves_settings.PreviewTargetGizmoColor;
                    curves_controller.PreviewTargetGizmoSize = curves_settings.PreviewTargetGizmoSize;
                    curves_controller.PreviewTargetForwardGizmoColor = curves_settings.PreviewTargetForwardGizmoColor;
                    curves_controller.PreviewTargetForwardGizmoSize = curves_settings.PreviewTargetForwardGizmoSize;
                    for (int index = 0; index < keys.Count; index++)
                    {
                        BezierCurveKeyData target_path_key = keys[index];
                        if (SelectedIndex == index)
                        {
                            Handles.color = curves_settings.PrimaryKeyTangentColor;
                            Handles.DrawLine(target_path_key.StartPosition + offset, target_path_key.StartTangent + offset, 3.0f);
                            Handles.color = curves_settings.SecondaryKeyTangentColor;
                            Handles.DrawLine(target_path_key.EndPosition + offset, target_path_key.EndTangent + offset, 3.0f);
                        }
                        Handles.DrawBezier(offset + target_path_key.StartPosition, offset + target_path_key.EndPosition, offset + target_path_key.StartTangent, offset + target_path_key.EndTangent, curves_settings.CurvesColor, null, curves_settings.CurveLineThickness);
                        Vector3 new_end_position = target_path_key.EndPosition;
                        if (SelectedIndex == index)
                        {
                            Vector3 new_start_position = (index == 0) ? Vector3.zero : Handles.PositionHandle(target_path_key.StartPosition + offset, Quaternion.identity) - offset;
                            new_end_position = Handles.PositionHandle(new_end_position + offset, Quaternion.identity) - offset;
                            Vector3 new_start_tangent = Handles.PositionHandle(target_path_key.StartTangent + offset, Quaternion.identity) - offset;
                            Vector3 new_end_tangent = Handles.PositionHandle(target_path_key.EndTangent + offset, Quaternion.identity) - offset;
                            if
                            (
                                (target_path_key.StartPosition != new_start_position) ||
                                (target_path_key.EndPosition != new_end_position) ||
                                (target_path_key.StartTangent != new_start_tangent) ||
                                (target_path_key.EndTangent != new_end_tangent)
                            )
                            {
                                Undo.RecordObject(curves_controller, "Modify curve properties");
                                BezierCurveUtilities.ApplyBezierCurveKeyDataProperties(keys, index, new_start_position, new_end_position, new_start_tangent, new_end_tangent, target_path_key.IsEndPositionConnected, target_path_key.IsEndTangentConnected);
                                serializedObject.ApplyModifiedPropertiesWithoutUndo();
                                RepaintWindow();
                            }
                        }
                        else
                        {
                            Handles.color = curves_settings.KeyColor;
                            Vector3 button_handle_position = offset + ((target_path_key.EndPosition - target_path_key.StartPosition) * 0.5f);
                            float handle_size = HandleUtility.GetHandleSize(button_handle_position) * curves_settings.KeyHandleSizeMultiplier;
                            Handles.Label(button_handle_position, $"<color=\"#{ primary_font_color_code }\">Key :</color> <color=\"#{ secondary_font_color_code }\">{ index }</color>", labelGUIStyle);
                            if (Handles.Button(button_handle_position, is_handle_cap_2d ? (Quaternion.Inverse(curves_controller.transform.rotation) * camera_rotation) : Quaternion.identity, handle_size, handle_size, handle_cap_function))
                            {
                                SelectedIndex = index;
                                RepaintWindow();
                            }
                        }
                        offset += new_end_position;
                    }
                    Handles.matrix = old_matrix;
                    Handles.color = old_color;
                }
            }
        }

        /// <summary>
        /// Gets color code from color
        /// </summary>
        /// <param name="color">Color</param>
        /// <returns>Color code</returns>
        private static string GetColorCode(Color32 color) => $"{color.r:X2}{color.g:X2}{color.b:X2}";
    }
}
