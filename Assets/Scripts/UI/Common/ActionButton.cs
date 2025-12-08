using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Common
{
    [RequireComponent(typeof(Button))]
    public class ActionButton : MonoBehaviour
    {
        public event Action Clicked;
        
        private Button _button;
        private bool _isInitialized;

        protected virtual void Awake()
        {
            if (!_isInitialized)
            {
                Initialize();
            }
        }

        protected virtual void OnDestroy()
        {
            _button.onClick.RemoveListener(OnClick);
        }

        public virtual void Initialize()
        {
            if (_isInitialized)
                return;
            
            _isInitialized = true;
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClick);
        }
        
        private void OnClick()
        {
            Clicked?.Invoke();
        }
    }
}