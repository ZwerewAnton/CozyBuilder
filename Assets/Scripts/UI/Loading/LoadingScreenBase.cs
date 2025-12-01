using System;
using System.Threading.Tasks;
using Configs;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace UI.Loading
{
    public abstract class LoadingScreenBase : MonoBehaviour
    {
        [SerializeField] protected CanvasGroup canvasGroup;
    
        private ApplicationConfigs _configs;
        private bool _isOn;

        [Inject]
        private void Construct(ApplicationConfigs configs)
        {
            _configs = configs;
        }

        protected void Awake()
        {
            Initialize();
        }

        protected virtual void Initialize()
        {
            SetProgress(0f);
        }

        public virtual void SetProgress(float value) { }

        public void ShowLoadingScreenImmediately()
        {
            if (_isOn) 
                return;

            gameObject.SetActive(true);
            SetProgress(0f);
            canvasGroup.alpha = 1f;
            _isOn = true;
        }

        public virtual async Task ShowLoadingScreenAsync()
        {
            if (_isOn) 
                return;

            gameObject.SetActive(true);
            SetProgress(0f);
            await FadeLoadingScreen(0f, 1f, () => _isOn = true)
                .AsyncWaitForCompletion();
        }

        public virtual async Task HideLoadingScreenAsync()
        {
            await FadeLoadingScreen(1f, 0f, HideLoadingScreen)
                .AsyncWaitForCompletion();
        }

        protected virtual void HideLoadingScreen()
        {
            gameObject.SetActive(false);
            _isOn = false;
        }

        protected virtual Tween FadeLoadingScreen(float startValue, float targetValue, Action completed = null)
        {
            canvasGroup.alpha = startValue;
            var duration = _configs.loadingScreenFadeTime;
            return canvasGroup.DOFade(targetValue, duration)
                .SetEase(Ease.Linear)
                .OnComplete(() => completed?.Invoke());
        }
    }
}