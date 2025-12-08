using UnityEngine;
using UnityEngine.UI;

namespace UI.Common
{
    [RequireComponent(typeof(Image))]
    public class ToggleButton : ActionButton
    {
        [SerializeField] private Sprite onImage;
        [SerializeField] private Sprite offImage;
        
        private Image _buttonImage;

        public override void Initialize()
        {
            base.Initialize();
            _buttonImage = GetComponent<Image>();
        }

        public void SetState(bool isOn)
        {
            _buttonImage.sprite = isOn ? onImage : offImage;
        }
    }
}