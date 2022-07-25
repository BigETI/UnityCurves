using UnityEngine;

/// <summary>
/// Unity Curves namespace
/// </summary>
namespace UnityCurves
{
    /// <summary>
    /// An interface that represents a curves controller
    /// </summary>
    public interface ICurvesController : IBaseCurvesController
    {
        /// <summary>
        /// Progress
        /// </summary>
        float Progress { get; set; }

        /// <summary>
        /// Target transform
        /// </summary>
        Transform TargetTransform { get; set; }

        /// <summary>
        /// Target state
        /// </summary>
        IApproximateBezierCurvePoint TargetState { get; }

#if UNITY_EDITOR
        /// <summary>
        /// Preview target gizmo type
        /// </summary>
        EGizmoType PreviewTargetGizmoType { get; set; }

        /// <summary>
        /// Preview target gizmo color
        /// </summary>
        Color PreviewTargetGizmoColor { get; set; }

        /// <summary>
        /// Preview target gizmo size
        /// </summary>
        float PreviewTargetGizmoSize { get; set; }

        /// <summary>
        /// Preview target forward gizmo color
        /// </summary>
        Color PreviewTargetForwardGizmoColor { get; set; }

        /// <summary>
        /// Preview target forward gizmo size
        /// </summary>
        float PreviewTargetForwardGizmoSize { get; set; }
#endif
    }
}
