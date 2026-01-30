using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Settings
{
    public class SettingsDropdownMenu : MonoBehaviour
    {
        [Header("Buttons")] [SerializeField] private Button settingButton;

        [SerializeField] private Button[] menuButtons;

        [Space] [Header("Spacing Between Menu Items")] [SerializeField]
        private Vector2 spacing = new(0f, -60f);

        [Space] [Header("Main Button Rotation")] [SerializeField]
        private float rotationDuration;

        [SerializeField] private Ease rotationEase = Ease.Linear;

        [Space] [Header("Animation")] [SerializeField]
        private float expandDuration;

        [SerializeField] private float collapseDuration;
        [SerializeField] private Ease expandEase = Ease.OutBack;
        [SerializeField] private Ease collapseEase = Ease.InBack;

        [Space] [Header("Fading")] [SerializeField]
        private float expandFadeDuration;

        [SerializeField] private float collapseFadeDuration;
        private Image[] _buttonsImage;
        private RectTransform[] _buttonsTransform;
        private bool _isExpanded;
        private int _itemsCount;

        private Vector2 _settingButtonPosition;

        private void Awake()
        {
            _buttonsTransform = new RectTransform[menuButtons.Length];
            _buttonsImage = new Image[menuButtons.Length];

            for (var i = 0; i < menuButtons.Length; i++)
            {
                _buttonsTransform[i] = menuButtons[i].gameObject.GetComponent<RectTransform>();
                _buttonsImage[i] = menuButtons[i].gameObject.GetComponent<Image>();
            }
        }

        private void Start()
        {
            settingButton = transform.GetChild(0).GetComponent<Button>();
            settingButton.transform.SetAsLastSibling();
            _settingButtonPosition = settingButton.GetComponent<RectTransform>().localPosition;
            _itemsCount = transform.childCount - 1;

            ResetButtonsPosition();
        }

        public void ToggleMenu()
        {
            AnimateButtons();
            _isExpanded = !_isExpanded;
        }

        private void AnimateButtons()
        {
            Tween.StopAll(settingButton.transform);

            for (var i = 0; i < _itemsCount; i++)
            {
                Tween.StopAll(_buttonsTransform[i]);
                Tween.StopAll(_buttonsImage[i]);

                Tween.LocalPosition(
                    _buttonsTransform[i],
                    _isExpanded ? _settingButtonPosition : _settingButtonPosition + spacing * (i + 1),
                    _isExpanded ? collapseDuration : expandDuration,
                    _isExpanded ? collapseEase : expandEase
                );
                Tween.Alpha(
                    _buttonsImage[i],
                    _isExpanded ? 1f : 0f,
                    _isExpanded ? 0f : 1f,
                    _isExpanded ? collapseFadeDuration : expandFadeDuration
                );
            }

            Tween.LocalRotation(
                settingButton.transform,
                Vector3.zero,
                Vector3.forward * 180f,
                rotationDuration,
                rotationEase
            );
        }

        private void ResetButtonsPosition()
        {
            for (var i = 0; i < _itemsCount; i++) _buttonsTransform[i].transform.localPosition = _settingButtonPosition;
        }
    }
}