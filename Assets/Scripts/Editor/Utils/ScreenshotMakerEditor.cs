using UnityEditor;
using UnityEngine;

namespace Utils.Editor
{
    [CustomEditor(typeof(ScreenshotMaker))]
    public class ScreenshotMakerEditor : UnityEditor.Editor 
    {
        public override void OnInspectorGUI() 
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Take Screenshot")) 
            {
                ((ScreenshotMaker)serializedObject.targetObject).TakeScreenshot();
            }
        }
    }
}