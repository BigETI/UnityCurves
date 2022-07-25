using System;
using System.Collections.Generic;
using UnityCurves.Data;
using UnityEngine;
using UnityParallel;
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
        private BezierCurveData path = new();

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
        /// Gets the approximate bezier curve point in world space
        /// </summary>
        /// <param name="localApproximateBezierCurvePoint">Local approximate bezier curve point</param>
        /// <returns>Approximate bezier curve point in world space</returns>
        public IApproximateBezierCurvePoint GetWorldApproximateBezierCurvePoint(IApproximateBezierCurvePoint localApproximateBezierCurvePoint) =>
            new ApproximateBezierCurvePoint(transform.TransformPoint(localApproximateBezierCurvePoint.Position), transform.TransformDirection(localApproximateBezierCurvePoint.Forward), transform.TransformDirection(localApproximateBezierCurvePoint.Up), localApproximateBezierCurvePoint.Progress);

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
        /// Gets an approximate Bézier curve point
        /// </summary>
        /// <param name="progress">Progress</param>
        /// <returns>Approximate Bézier curve point</returns>
        public IApproximateBezierCurvePoint GetApproximateBezierCurvePoint(float progress)
        {
            if ((Path.Keys.Count * pathKeySegmentCount) != bakedPathKeySegments.Length)
            {
                BakeTargetPathKeySegments();
            }
            return GetWorldApproximateBezierCurvePoint(BezierCurveUtilities.GetApproximateBezierCurveObjectState(BakedPathKeySegments, progress, progressWrapMode, isInterpolatingForwardVector));
        }

        /// <summary>
        /// Gets the closest approximate point to bezier curve
        /// </summary>
        /// <param name="position">Position</param>
        /// <returns>Closest approximate point to bezier curve</returns>
        public IApproximateBezierCurvePoint GetClosestApproximatePointToBezierCurve(Vector3 position)
        {
            if ((Path.Keys.Count * pathKeySegmentCount) != bakedPathKeySegments.Length)
            {
                BakeTargetPathKeySegments();
            }
            return GetWorldApproximateBezierCurvePoint(BezierCurveUtilities.GetClosestPointToBezierCurve(BakedPathKeySegments.Segments, transform.InverseTransformPoint(position)));
        }

        /// <summary>
        /// Gets invoked when script has been started
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
