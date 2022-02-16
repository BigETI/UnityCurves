using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Unity Curves data namespace
/// </summary>
namespace UnityCurves.Data
{
    /// <summary>
    /// A class that describes bezier curve data
    /// </summary>
    [Serializable]
    public class BezierCurveData : IBezierCurveData
    {
        /// <summary>
        /// Keys
        /// </summary>
        [SerializeField]
        private List<BezierCurveKeyData> keys = new List<BezierCurveKeyData>()
        {
            new BezierCurveKeyData(Vector3.zero, Vector3.right, Vector3.right * 0.5f, Vector3.right * 0.5f, true, true)
        };

        /// <summary>
        /// Keys
        /// </summary>
        public List<BezierCurveKeyData> Keys
        {
            get => keys ??= new List<BezierCurveKeyData>();
            set => keys = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}
