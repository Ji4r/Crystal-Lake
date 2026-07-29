using UnityEditor;
using UnityEditor.Overlays;

namespace Weit.SceneSwitcher
{
    [Overlay(typeof(SceneView), "SceneSelector")]
    public class SceneSelectorOverlay : ToolbarOverlay
    {
        public SceneSelectorOverlay() : base("Weit/SceneSelector")
        {
        }
    }
}