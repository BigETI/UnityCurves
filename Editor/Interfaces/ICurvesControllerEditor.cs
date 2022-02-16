using UnityPatternsEditor;

/// <summary>
/// Unity Curves editor namespace
/// </summary>
namespace UnityCurvesEditor
{
    /// <summary>
    /// An interface that represents a curves controller editor
    /// </summary>
    public interface ICurvesControllerEditor : IEditor
    {
        /// <summary>
        /// Selected index
        /// </summary>
        int SelectedIndex { get; set; }
    }
}
