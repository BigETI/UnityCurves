using System;
using System.Collections.Generic;

/// <summary>
/// Unity Curves namespace
/// </summary>
namespace UnityCurves
{
    /// <summary>
    /// A structure that describes approximate Bézier curve key segments
    /// </summary>
    public readonly struct ApproximateBezierCurveKeySegments : IApproximateBezierCurveKeySegments
    {
        /// <summary>
        /// Bézier curve key segments
        /// </summary>
        public IReadOnlyList<IApproximateBezierCurveKeySegment> Segments { get; }

        /// <summary>
        /// Length
        /// </summary>
        public float Length { get; }

        /// <summary>
        /// Constructs new Bézier curve key segments
        /// </summary>
        /// <param name="segments">Bézier curve key segments</param>
        public ApproximateBezierCurveKeySegments(IReadOnlyList<IApproximateBezierCurveKeySegment> segments)
        {
            Segments = segments ?? throw new ArgumentNullException(nameof(segments));
            Length = BezierCurveUtilities.GetApproximateLength(segments);
        }
    }
}
