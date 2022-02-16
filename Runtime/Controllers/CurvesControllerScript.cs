using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityCurves.Data;
using UnityEngine;
using UnityPatterns.Controllers;

/// <summary>
/// Unity Curves controller namespace
/// </summary>
namespace UnityCurves.Controllers
{
    /// <summary>
    /// A class that describes a curves controller script
    /// </summary>
    public class CurvesControllerScript : AControllerScript, ICurvesController
    {
        /// <summary>
        /// Path key segment count
        /// </summary>
        [SerializeField]
        private uint pathKeySegmentCount = 32U;

        /// <summary>
        /// Progress
        /// </summary>
        [SerializeField]
        [Min(0.0f)]
        private float progress;

        /// <summary>
        /// Progress wrap mode
        /// </summary>
        [SerializeField]
        private EProgressWrapMode progressWrapMode;

        /// <summary>
        /// Is interpolating forward vector
        /// </summary>
        [SerializeField]
        private bool isInterpolatingForwardVector = true;

        /// <summary>
        /// Target transform
        /// </summary>
        [SerializeField]
        private Transform targetTransform = default;

        /// <summary>
        /// Target path
        /// </summary>
        [SerializeField]
        private BezierCurveData targetPath = new BezierCurveData();

        /// <summary>
        /// Baked target path key segments
        /// </summary>
        private IApproximateBezierCurveKeySegment[] bakedTargetPathKeySegments = Array.Empty<IApproximateBezierCurveKeySegment>();

        /// <summary>
        /// Path key segment count
        /// </summary>
        public uint PathKeySegmentCount
        {
            get => PathKeySegmentCount;
            set => pathKeySegmentCount = (uint)Mathf.Max((int)value, 1U);
        }

        /// <summary>
        /// Progress
        /// </summary>
        public float Progress
        {
            get => progress;
            set => progress = Mathf.Max(value, 0.0f);
        }

        /// <summary>
        /// Progress wrap mode
        /// </summary>
        public EProgressWrapMode ProgressWrapMode
        {
            get => progressWrapMode;
            set => progressWrapMode = value;
        }

        /// <summary>
        /// Is interpolating forward vector
        /// </summary>
        public bool IsInterpolatingForwardVector
        {
            get => isInterpolatingForwardVector;
            set => isInterpolatingForwardVector = value;
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
        /// Target path
        /// </summary>
        public BezierCurveData TargetPath
        {
            get => targetPath ??= new BezierCurveData();
            set => targetPath = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Target state
        /// </summary>
        public IApproximateBezierCurveObjectState TargetState
        {
            get
            {
                if ((TargetPath.Keys.Count * pathKeySegmentCount) != bakedTargetPathKeySegments.Length)
                {
                    BakeTargetPathKeySegments();
                }
                IApproximateBezierCurveObjectState approximate_bezier_curve_object_state = BezierCurveUtilities.GetApproximateBezierCurveObjectState(BakedTargetPathKeySegments, progress, progressWrapMode, isInterpolatingForwardVector);
                return new ApproximateBezierCurveObjectState(transform.TransformPoint(approximate_bezier_curve_object_state.Position), transform.TransformDirection(approximate_bezier_curve_object_state.Forward));
            }
        }

        /// <summary>
        /// Baked target path key segments
        /// </summary>
        public IApproximateBezierCurveKeySegments BakedTargetPathKeySegments { get; private set; } = new ApproximateBezierCurveKeySegments(Array.Empty<IApproximateBezierCurveKeySegment>());

        /// <summary>
        /// Bakes target path key segments
        /// </summary>
        private void BakeTargetPathKeySegments()
        {
            IReadOnlyList<BezierCurveKeyData> keys = TargetPath.Keys;
            uint segment_count = (uint)keys.Count * pathKeySegmentCount;
            if (bakedTargetPathKeySegments.Length != segment_count)
            {
                Array.Resize(ref bakedTargetPathKeySegments, (int)segment_count);
            }
            if (bakedTargetPathKeySegments.Length > 0)
            {
                Parallel.For(0, keys.Count, (index) => keys[index].GetApproximateSegments(bakedTargetPathKeySegments, (uint)index * pathKeySegmentCount, pathKeySegmentCount));
                BakedTargetPathKeySegments = new ApproximateBezierCurveKeySegments(bakedTargetPathKeySegments);
            }
        }

        /// <summary>
        /// Gets invoked when script gets started
        /// </summary>
        protected virtual void Start()
        {
            if (!targetTransform)
            {
                Debug.LogError("Please assign a target transform to this component", this);
            }
            else if (targetTransform == transform)
            {
                Debug.LogError("Please do not assign its own transform as a target transform to this component", this);
            }
            BakeTargetPathKeySegments();
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
        protected virtual void OnValidate()
        {
            pathKeySegmentCount = (uint)Mathf.Max((int)pathKeySegmentCount, 1U);
            if (targetTransform && (targetTransform == transform))
            {
                targetTransform = null;
            }
            BakeTargetPathKeySegments();
        }

        /// <summary>
        /// Gets invoked when gizmos need to be drawn, when game object has been selected
        /// </summary>
        protected virtual void OnDrawGizmosSelected()
        {
            Color old_color = Gizmos.color;
            Gizmos.color = Color.cyan;
            IApproximateBezierCurveObjectState target_state = TargetState;
            Gizmos.DrawLine(target_state.Position, target_state.Position + (target_state.Forward * 10.0f));
            Gizmos.DrawSphere(target_state.Position, 1.0f);
            Gizmos.color = old_color;
        }
#endif
    }
}
