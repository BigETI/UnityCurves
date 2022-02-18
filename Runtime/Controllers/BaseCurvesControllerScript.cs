using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityCurves.Data;
using UnityEngine;
using UnityPatterns.Controllers;

/// <summary>
/// Unity Curves controllers namespace
/// </summary>
namespace UnityCurves.Controllers
{
    /// <summary>
    /// A class that describes a base curves controller script
    /// </summary>
    public class BaseCurvesControllerScript : AControllerScript, IBaseCurvesController
    {
        /// <summary>
        /// Path key segment count
        /// </summary>
        [SerializeField]
        private uint pathKeySegmentCount = 32U;

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
        /// Path
        /// </summary>
        [SerializeField]
        private BezierCurveData path = new BezierCurveData();

        /// <summary>
        /// Baked path key segments
        /// </summary>
        private IApproximateBezierCurveKeySegment[] bakedPathKeySegments = Array.Empty<IApproximateBezierCurveKeySegment>();

        /// <summary>
        /// Path key segment count
        /// </summary>
        public uint PathKeySegmentCount
        {
            get => PathKeySegmentCount;
            set => pathKeySegmentCount = (uint)Mathf.Max((int)value, 1U);
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
        /// Path
        /// </summary>
        public BezierCurveData Path
        {
            get => path ??= new BezierCurveData();
            set => path = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Baked target path key segments
        /// </summary>
        public IApproximateBezierCurveKeySegments BakedPathKeySegments { get; private set; } = new ApproximateBezierCurveKeySegments(Array.Empty<IApproximateBezierCurveKeySegment>());

        /// <summary>
        /// Bakes target path key segments
        /// </summary>
        private void BakeTargetPathKeySegments()
        {
            IReadOnlyList<BezierCurveKeyData> keys = Path.Keys;
            uint segment_count = (uint)keys.Count * pathKeySegmentCount;
            if (bakedPathKeySegments.Length != segment_count)
            {
                Array.Resize(ref bakedPathKeySegments, (int)segment_count);
            }
            if (bakedPathKeySegments.Length > 0)
            {
                Parallel.For(0, keys.Count, (index) => keys[index].GetApproximateSegments(bakedPathKeySegments, (uint)index * pathKeySegmentCount, pathKeySegmentCount));
                BakedPathKeySegments = new ApproximateBezierCurveKeySegments(bakedPathKeySegments);
            }
        }

        /// <summary>
        /// Gets Bézier curve object state
        /// </summary>
        /// <param name="progress">Progress</param>
        /// <returns>Bézier curve object state</returns>
        public IApproximateBezierCurveObjectState GetBezierCurveObjectState(float progress)
        {
            if ((Path.Keys.Count * pathKeySegmentCount) != bakedPathKeySegments.Length)
            {
                BakeTargetPathKeySegments();
            }
            IApproximateBezierCurveObjectState approximate_bezier_curve_object_state = BezierCurveUtilities.GetApproximateBezierCurveObjectState(BakedPathKeySegments, progress, progressWrapMode, isInterpolatingForwardVector);
            return new ApproximateBezierCurveObjectState(transform.TransformPoint(approximate_bezier_curve_object_state.Position), transform.TransformDirection(approximate_bezier_curve_object_state.Forward));
        }

        /// <summary>
        /// Gets invoked when script gets started
        /// </summary>
        protected virtual void Start() => BakeTargetPathKeySegments();

#if UNITY_EDITOR
        /// <summary>
        /// Gets invoked when script needs to be validated
        /// </summary>
        protected virtual void OnValidate()
        {
            pathKeySegmentCount = (uint)Mathf.Max((int)pathKeySegmentCount, 1U);
            BakeTargetPathKeySegments();
        }
#endif
    }
}
