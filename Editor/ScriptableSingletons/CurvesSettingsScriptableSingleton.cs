using UnityEditor;
using UnityEngine;

/// <summary>
/// Unity Curves editor namespace
/// </summary>
namespace UnityCurvesEditor
{
    /// <summary>
    /// A class that describes curves settings scriptable singleton
    /// </summary>
    [FilePath("BigETI/UnityCurves/Curves.asset", FilePathAttribute.Location.PreferencesFolder)]
    public class CurvesSettingsScriptableSingleton : ScriptableSingleton<CurvesSettingsScriptableSingleton>, ICurvesSettingsScriptableSingleton
    {
        /// <summary>
        /// Curves color
        /// </summary>
        [SerializeField]
        private Color curvesColor = Color.red;

        /// <summary>
        /// Curve line thickness
        /// </summary>
        [SerializeField]
        [Min(0.0f)]
        private float curveLineThickness = 10.0f;

        /// <summary>
        /// Primary font color
        /// </summary>
        [SerializeField]
        private Color32 primaryFontColor = new Color32(0xFF, 0xFF, 0xFF, 0xFF);

        /// <summary>
        /// Secondary font color
        /// </summary>
        [SerializeField]
        private Color32 secondaryFontColor = new Color32(0x0, 0xFF, 0xFF, 0xFF);

        /// <summary>
        /// Handle cap color
        /// </summary>
        [SerializeField]
        private EHandleCap keyHandleCap = EHandleCap.Rectangle;

        /// <summary>
        /// Key color
        /// </summary>
        [SerializeField]
        private Color keyColor = Color.cyan;

        /// <summary>
        /// Key handle size multiplier
        /// </summary>
        [SerializeField]
        [Min(0.0f)]
        private float keyHandleSizeMultiplier = 0.25f;

        /// <summary>
        /// Primary key tangent color
        /// </summary>
        [SerializeField]
        private Color primaryKeyTangentColor = Color.green;

        /// <summary>
        /// Secondary key tangent color
        /// </summary>
        [SerializeField]
        private Color secondaryKeyTangentColor = Color.cyan;

        /// <summary>
        /// Curves color
        /// </summary>
        public Color CurvesColor
        {
            get => curvesColor;
            set
            {
                if (curvesColor != value)
                {
                    curvesColor = value;
                    Save(true);
                }
            }
        }

        /// <summary>
        /// Curve line thickness
        /// </summary>
        public float CurveLineThickness
        {
            get => curveLineThickness;
            set
            {
                float new_curve_line_thickness = Mathf.Max(value, 0.0f);
                if (curveLineThickness != new_curve_line_thickness)
                {
                    curveLineThickness = new_curve_line_thickness;
                    Save(true);
                }
            }
        }

        /// <summary>
        /// Primary font color
        /// </summary>
        public Color32 PrimaryFontColor
        {
            get => primaryFontColor;
            set
            {
                if
                (
                    (primaryFontColor.r != value.r) ||
                    (primaryFontColor.g != value.g) ||
                    (primaryFontColor.b != value.b)
                )
                {
                    primaryFontColor = value;
                    Save(true);
                }
            }
        }

        /// <summary>
        /// Secondary font color
        /// </summary>
        public Color32 SecondaryFontColor
        {
            get => secondaryFontColor;
            set
            {
                if
                (
                    (secondaryFontColor.r != value.r) ||
                    (secondaryFontColor.g != value.g) ||
                    (secondaryFontColor.b != value.b)
                )
                {
                    secondaryFontColor = value;
                    Save(true);
                }
            }
        }

        /// <summary>
        /// Handle cap color
        /// </summary>
        public EHandleCap KeyHandleCap
        {
            get => keyHandleCap;
            set
            {
                if (keyHandleCap != value)
                {
                    keyHandleCap = value;
                    Save(true);
                }
            }
        }

        /// <summary>
        /// Key color
        /// </summary>
        public Color KeyColor
        {
            get => keyColor;
            set
            {
                if (keyColor != value)
                {
                    keyColor = value;
                    Save(true);
                }
            }
        }

        /// <summary>
        /// Key handle size multiplier
        /// </summary>
        public float KeyHandleSizeMultiplier
        {
            get => keyHandleSizeMultiplier;
            set
            {
                float new_key_handle_size_multiplier = Mathf.Max(value, 0.0f);
                if (keyHandleSizeMultiplier != new_key_handle_size_multiplier)
                {
                    keyHandleSizeMultiplier = new_key_handle_size_multiplier;
                    Save(true);
                }
            }
        }

        /// <summary>
        /// Primary key tangent color
        /// </summary>
        public Color PrimaryKeyTangentColor
        {
            get => primaryKeyTangentColor;
            set
            {
                if (primaryKeyTangentColor != value)
                {
                    primaryKeyTangentColor = value;
                    Save(true);
                }
            }
        }

        /// <summary>
        /// Secondary key tangent color
        /// </summary>
        public Color SecondaryKeyTangentColor
        {
            get => secondaryKeyTangentColor;
            set
            {
                if (secondaryKeyTangentColor != value)
                {
                    secondaryKeyTangentColor = value;
                    Save(true);
                }
            }
        }
    }
}
