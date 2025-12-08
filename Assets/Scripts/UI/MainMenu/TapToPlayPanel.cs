using System;
using UI.Common;
using UI.Settings;
using UnityEngine;

namespace UI.MainMenu
{
    [RequireComponent(typeof(ActionButton))]
    public class TapToPlayPanel : MonoBehaviour
    {
        private ActionButton _tapToPlayButton;
        
        public event Action Clicked
        {
            add => _tapToPlayButton.Clicked += value;
            remove => _tapToPlayButton.Clicked -= value;
        }
        
        private void Awake()
        {
            _tapToPlayButton = GetComponent<ActionButton>();
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}