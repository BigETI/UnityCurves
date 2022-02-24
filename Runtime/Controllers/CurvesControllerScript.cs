using UnityEngine;

/// <summary>
/// Unity Curves controller namespace
/// </summary>
namespace UnityCurves.Controllers
{
    /// <summary>
    /// A class that describes a curves controller script
    /// </summary>
    public class CurvesControllerScript : BaseCurvesControllerScript, ICurvesController
    {
        /// <summary>
        /// Progress
        /// </summary>
        [SerializeField]
        [Min(0.0f)]
        private float progress;

        /// <summary>
        /// Target transform
        /// </summary>
        [SerializeField]
        private Transform targetTransform = default;

        /// <summary>
        /// Progress
        /// </summary>
        public float Progress
        {
            get => progress;
            set => progress = Mathf.Max(value, 0.0f);
        }

        /// <summary>
        /// Target transform
        /// </summary>
        public Transform TargetTransform
        {
            get => targetTransform;
            set => targetTransform = value;
        }

        /// <summary>
        /// Target state
        /// </summary>
        public IApproximateBezierCurveObjectState TargetState => GetBezierCurveObjectState(progress);

#if UNITY_EDITOR
        /// <summary>
        /// Preview target gizmo type
        /// </summary>
        public EGizmoType PreviewTargetGizmoType { get; set; } = EGizmoType.Sphere;

        /// <summary>
        /// Preview target gizmo color
        /// </summary>
        public Color PreviewTargetGizmoColor { get; set; } = Color.cyan;

        /// <summary>
        /// Preview target gizmo size
        /// </summary>
        public float PreviewTargetGizmoSize { get; set; } = 1.0f;

        /// <summary>
        /// Preview target forward gizmo color
        /// </summary>
        public Color PreviewTargetForwardGizmoColor { get; set; } = Color.cyan;

        /// <summary>
        /// Preview target forward gizmo size
        /// </summary>
        public float PreviewTargetForwardGizmoSize { get; set; } = 10.0f;
#endif

        /// <summary>
        /// Gets invoked when script has been started
        /// </summary>
        protected override void Start()
        {
            base.Start();
            if (!targetTransform)
            {
                Debug.LogError("Please assign a target transform to this component", this);
            }
            else if (targetTransform == transform)
            {
                Debug.LogError("Please do not assign its own transform as a target transform to this component", this);
            }
        }

        /// <summary>
        /// Gets invoked when script perfoms a physics update
        /// </summary>
        protected virtual void FixedUpdate()
        {
            if (targetTransform)
            {
                IApproximateBezierCurveObjectState target_state = TargetState;
                targetTransform.position = target_state.Position;
                targetTransform.LookAt(target_state.Position + target_state.Forward, Vector3.up);
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Gets invoked when script needs to be validated
        /// </summary>
        protected override void OnValidate()
        {
            base.OnValidate();
            if (targetTransform && (targetTransform == transform))
            {
                targetTransform = null;
            }
        }

        /// <summary>
        /// Gets invoked when gizmos need to be drawn, when game object has been selected
        /// </summary>
        protected virtual void OnDrawGizmosSelected()
        {
            Color old_color = Gizmos.color;
            Gizmos.color = PreviewTargetForwardGizmoColor;
            IApproximateBezierCurveObjectState target_state = TargetState;
            Gizmos.DrawLine(target_state.Position, target_state.Position + (target_state.Forward * PreviewTargetForwardGizmoSize));
            Gizmos.color = PreviewTargetGizmoColor;
            switch (PreviewTargetGizmoType)
            {
                case EGizmoType.Cube:
                    Gizmos.DrawCube(target_state.Position, Vector3.one * PreviewTargetGizmoSize);
                    break;
                case EGizmoType.Sphere:
                    Gizmos.DrawSphere(target_state.Position, PreviewTargetGizmoSize);
                    break;
                case EGizmoType.WireCube:
                    Gizmos.DrawWireCube(target_state.Position, Vector3.one * PreviewTargetGizmoSize);
                    break;
                case EGizmoType.WireSphere:
                    Gizmos.DrawWireSphere(target_state.Position, PreviewTargetGizmoSize);
                    break;
            }
            Gizmos.color = old_color;
        }
#endif
    }
}
