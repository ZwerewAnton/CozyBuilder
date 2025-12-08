using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;
using Utils.BootstrapLoading;

namespace Editor.SceneManagement
{
    [InitializeOnLoad]
    public class SceneSwitcherPanel
    {
        static SceneSwitcherPanel()
        {
            ToolbarExtender.RightToolbarGUI.Add(OnToolbarGUI);
        }

        private static void OnToolbarGUI()
        {
            GUILayout.FlexibleSpace();
            
            if (GUILayout.Button(new GUIContent("Boot", "Start Scene Boot")))
            {
                GoToBootstrapScene();
            }

            if (GUILayout.Button(new GUIContent("Menu", "Start Scene MainMenu")))
            {
                GoToMainMenuScene();
            }

            if (GUILayout.Button(new GUIContent("Level", "Start Scene Level")))
            {
                GoToGameScene();
            }
        }
    
        private static void GoToBootstrapScene()
        {
            SceneSwitcherEditor.OpenSceneSingle(SceneConfigEditor.BootSceneName);
        }

        private static void GoToMainMenuScene()
        {
            SceneSwitcherEditor.OpenSceneSingle(SceneConfigEditor.MenuSceneName);
        }

        private static void GoToGameScene()
        {
            SceneSwitcherEditor.OpenSceneSingle(SceneConfigEditor.GameSceneName);
        }
    }
}