using System.IO;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEditor.SceneManagement;

namespace Weit.SceneSwitcher
{
    [EditorToolbarElement("Weit/SceneSelector", typeof(SceneView))]
    public class SceneSelector : EditorToolbarDropdown
    {
        private const string SceneFolderKey = "Weit.SceneSwitcher.SceneFolder";

        public SceneSelector()
        {
            text = "SceneSelector";

            clicked += ShowMenu;
        }

        private void ShowMenu()
        {
            var menu = new GenericMenu();

            string folder = "Assets";

            if (SceneSwitcherSettings.Instance.ScenesFolder != null)
            {
                folder = AssetDatabase.GetAssetPath(
                    SceneSwitcherSettings.Instance.ScenesFolder);
            }

            if (!AssetDatabase.IsValidFolder(folder))
                folder = "Assets";

            string[] guids = AssetDatabase.FindAssets("t:Scene", new[] { folder });

            foreach (string guid in guids)
            {
                string scenePath = AssetDatabase.GUIDToAssetPath(guid);

                menu.AddItem(
                    new GUIContent(Path.GetFileNameWithoutExtension(scenePath)),
                    false,
                    () =>
                    {
                        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                        {
                            EditorSceneManager.OpenScene(scenePath);
                        }
                    });
            }

            menu.ShowAsContext();
        }
    }
}