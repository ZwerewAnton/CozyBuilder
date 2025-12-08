using System;
using System.IO;
using UnityEngine;

namespace Utils
{
    public class ScreenshotMaker : MonoBehaviour
    {
        [SerializeField] private int width = 2160;
        [SerializeField] private int height = 1080;
        [SerializeField] private string folder = "Screenshots";
        [SerializeField] private string filenamePrefix = "screenshot";
        [SerializeField] private bool ensureTransparentBackground;

        [ContextMenu("Take Screenshot")]
        public void TakeScreenshot()
        {
            folder = GetSafePath(folder.Trim('/'));
            filenamePrefix = GetSafeFilename(filenamePrefix);

            var dir = Application.dataPath + "/" + folder + "/";
            var filename = filenamePrefix + "_" + DateTime.Now.ToString("yyMMdd_HHmmss") + ".png";
            var path = dir + filename;

            var cam = GetComponent<Camera>();

            var rt = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
            cam.targetTexture = rt;
        
            var clearFlags = cam.clearFlags;
            var backgroundColor = cam.backgroundColor;
            if (ensureTransparentBackground)
            {
                cam.clearFlags = CameraClearFlags.SolidColor;
                cam.backgroundColor = new Color();
            }
            cam.Render();

            if (ensureTransparentBackground)
            {
                cam.clearFlags = clearFlags;
                cam.backgroundColor = backgroundColor;
            }

            var currentRT = RenderTexture.active;
            RenderTexture.active = cam.targetTexture;
        
            var screenshot = new Texture2D(width, height, TextureFormat.ARGB32, false);
            screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0, false);

            if (QualitySettings.activeColorSpace == ColorSpace.Linear) 
            {
                var pixels = screenshot.GetPixels();
                for (var p = 0; p < pixels.Length; p++)
                {
                    pixels[p] = pixels[p].gamma;
                }
                screenshot.SetPixels(pixels);
            }

            screenshot.Apply(false);

            Directory.CreateDirectory(dir);
            var png = screenshot.EncodeToPNG();
            File.WriteAllBytes(path, png);
        
            cam.targetTexture = null;
            RenderTexture.active = currentRT;
            Debug.Log("Screenshot saved to:\n" + path);
        }

        private static string GetSafePath(string path) 
        {
            return string.Join("_", path.Split(Path.GetInvalidPathChars()));
        }

        private static string GetSafeFilename(string filename) 
        {
            return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
        }
    }
}
