using System.Collections.Generic;
using UnityCurves.Data;

/// <summary>
/// Unity Curves namespace
/// </summary>
namespace UnityCurves
{
    /// <summary>
    /// An interface that represents bezier curve data
    /// </summary>
    public interface IBezierCurveData
    {
        /// <summary>
        /// Keys
        /// </summary>
        List<BezierCurveKeyData> Keys { get; set; }
    }
}
