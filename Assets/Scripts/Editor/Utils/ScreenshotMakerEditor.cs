using UnityEditor;
using UnityEngine;
using Utils;

namespace Editor.Utils
{
    [CustomEditor(typeof(ScreenshotMaker))]
    public class ScreenshotMakerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Take Screenshot")) ((ScreenshotMaker)serializedObject.targetObject).TakeScreenshot();
        }
    }
}