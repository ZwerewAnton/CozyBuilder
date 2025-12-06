using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Utils.BootstrapLoading;

namespace Editor.SceneManagement
{
#if !UNITY_INCLUDE_TESTS
    [InitializeOnLoad]
    public class EditorInit
    {
        private static readonly SceneAsset BootScene;
        
        static EditorInit()
        {
            var guids = AssetDatabase.FindAssets("t:scene " + SceneConfigEditor.BootSceneName, null);
            if (guids.Length == 0)
            {
                Debug.LogWarning("Couldn't find boot scene file");
            }
            else
            {
                var scenePath = AssetDatabase.GUIDToAssetPath(guids[0]);
                BootScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
                EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            }
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                var activeScene = EditorSceneManager.GetActiveScene();
                EditorPrefs.SetString(SceneConfigEditor.StartScenePrefsKey, activeScene.name);
                EditorSceneManager.playModeStartScene = BootScene;
            }
        }
    }
#endif
}