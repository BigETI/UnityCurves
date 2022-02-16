using System.Collections.Generic;

/// <summary>
/// Unity Curves namespace
/// </summary>
namespace UnityCurves
{
    /// <summary>
    /// An interface that represents approximate Bézier curve key segments
    /// </summary>
    public interface IApproximateBezierCurveKeySegments
    {
        /// <summary>
        /// Bézier curve key segments
        /// </summary>
        IReadOnlyList<IApproximateBezierCurveKeySegment> Segments { get; }

        /// <summary>
        /// Length
        /// </summary>
        float Length { get; }
    }
}
