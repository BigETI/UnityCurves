using UnityEngine;
using UnityPatternsEditor;

/// <summary>
/// Unity Curves editor namespace
/// </summary>
namespace UnityCurvesEditor
{
    /// <summary>
    /// An interface that represents a curves controller editor window
    /// </summary>
    public interface ICurvesControllerEditorWindow : IEditorWindow
    {
        /// <summary>
        /// Scroll position
        /// </summary>
        Vector2 ScrollPosition { get; set; }

        /// <summary>
        /// Is showing absolute positions
        /// </summary>
        bool IsShowingAbsolutePositions { get; set; }
    }
}
