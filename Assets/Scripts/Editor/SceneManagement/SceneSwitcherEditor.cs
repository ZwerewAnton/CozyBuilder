using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Editor.SceneManagement
{
    public static class SceneSwitcherEditor
    {
        public static void OpenSceneSingle(string sceneName)
        {
            if (EditorApplication.isPlaying)
            {
                Debug.LogWarning("Cannot be performed in Play mode");
                return;
            }

            EditorSceneManager.SaveOpenScenes();
            OpenSceneByName(sceneName);
        }

        public static void OpenSceneAdditionally(string sceneName)
        {
            if (EditorApplication.isPlaying)
            {
                Debug.LogWarning("Cannot be performed in Play mode");
                return;
            }

            EditorSceneManager.SaveOpenScenes();
            OpenSceneByName("Boot");
            OpenSceneByName(sceneName, OpenSceneMode.Additive);
        }

        private static void OpenSceneByName(string sceneName, OpenSceneMode mode = OpenSceneMode.Single)
        {
            var guids = AssetDatabase.FindAssets("t:scene " + sceneName, null);
            if (guids.Length == 0)
            {
                Debug.LogWarning("Couldn't find scene file");
            }
            else
            {
                var scenePath = AssetDatabase.GUIDToAssetPath(guids[0]);
                EditorSceneManager.OpenScene(scenePath, mode);
            }
        }
    }
}