using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI.Settings
{
    [RequireComponent(typeof(Image))]
    public class SettingsDropdownButton : MonoBehaviour
    {
        [FormerlySerializedAs("img")] [HideInInspector]
        public Image buttonImage;

        [FormerlySerializedAs("offImg")] public Sprite offImage;
        [FormerlySerializedAs("onImg")] public Sprite onImage;

        private bool _isOn;

        private void Awake()
        {
            buttonImage = GetComponent<Image>();
        }

        private void Start()
        {
            ChangeIcon(!_isOn);
        }

        public void SetToggle(bool value)
        {
            _isOn = value;
            ChangeIcon(_isOn);
        }

        public void ToggleAudio()
        {
            ChangeIcon(_isOn);
        }

        private void ChangeIcon(bool isOn)
        {
            buttonImage.sprite = isOn ? offImage : onImage;
            _isOn = !isOn;
        }
    }
}