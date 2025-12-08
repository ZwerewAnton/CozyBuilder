using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Utils
{
    public class FPSCounter : MonoBehaviour
    {
        [FormerlySerializedAs("_fpsText")] [SerializeField] 
        private Text fpsText;
        [FormerlySerializedAs("_dpiText")] [SerializeField] 
        private Text dpiText;
        [FormerlySerializedAs("_eventText")] [SerializeField] 
        private Text eventText;
        [FormerlySerializedAs("_hudRefreshRate")] [SerializeField] 
        private float hudRefreshRate = 1f;
 
        private float _timer;
        private void Start()
        {
            dpiText.text = "PPI: " + Screen.dpi;
        }
    
        private void Update()
        {
            if (UnityEngine.Input.touchCount > 0)
            {
                eventText.text = UnityEngine.Input.GetTouch(0).deltaPosition + " " + UnityEngine.Input.GetTouch(0).phase;
            }
            if (Time.unscaledTime > _timer)
            {
                var fps = (int)(1f / Time.unscaledDeltaTime);
                fpsText.text = "FPS: " + fps;
                _timer = Time.unscaledTime + hudRefreshRate;
            }
        }
    }
}