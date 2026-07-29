using UnityEditor;

namespace Weit.SceneSwitcher
{
    [FilePath("ProjectSettings/WeitSceneSwitcher.asset", FilePathAttribute.Location.ProjectFolder)]
    public class SceneSwitcherSettings : ScriptableSingleton<SceneSwitcherSettings>
    {
        public DefaultAsset ScenesFolder;

        public static SceneSwitcherSettings Instance => instance;

        public void Save()
        {
            Save(true);
        }
    }
}