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
        private List<BezierCurveKeyData> keys = new()
        {
            new(Vector3.zero, Vector3.right, 0.0f, 0.0f, Vector3.right * 0.5f, Vector3.right * 0.5f, true, true, true)
        };

        /// <summary>
        /// Keys
        /// </summary>
        public List<BezierCurveKeyData> Keys
        {
            get => keys ??= new();
            set => keys = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}
