using UI.Common;
using UnityEngine;

namespace UI.MainMenu
{
    public class ResetCameraButton : ActionButton
    {
        protected override void Awake()
        {
            base.Awake();

            gameObject.SetActive(false);
        }

        public void SetButtonVisibility(float cameraDesiredHeight, float cameraDesiredZoom)
        {
            var isVisible =
                !(Mathf.Approximately(cameraDesiredHeight, 0f) && Mathf.Approximately(cameraDesiredZoom, 0f));
            gameObject.SetActive(isVisible);
        }
    }
}