using System.IO;
using UnityEditor;
using UnityEngine;

namespace Weit.SceneSwitcher
{
    public static class SceneSwitcherMenu
    {
        private const string SceneFolderKey = "Weit.SceneSwitcher.SceneFolder";

        [MenuItem("Tools/Weit/SceneSwitcher/SetPathScene")]
        private static void SetPathScene()
        {
            string path = EditorUtility.OpenFolderPanel(
                "Select Scene Folder",
                Application.dataPath,
                "");

            if (string.IsNullOrEmpty(path))
                return;

            string projectPath = Path.GetFullPath(Application.dataPath + "/..");
            path = Path.GetFullPath(path);

            if (!path.StartsWith(projectPath))
            {
                EditorUtility.DisplayDialog(
                    "Invalid Folder",
                    "Folder must be inside the current Unity project.",
                    "OK");
                return;
            }

            string relativePath = path.Substring(projectPath.Length).Replace('\\', '/');


            if (relativePath.StartsWith("/"))
                relativePath = relativePath.Substring(1);

            SceneSwitcherSettings.Instance.ScenesFolder = AssetDatabase.LoadAssetAtPath<DefaultAsset>(relativePath);
            SceneSwitcherSettings.Instance.Save();
        }
    }
}