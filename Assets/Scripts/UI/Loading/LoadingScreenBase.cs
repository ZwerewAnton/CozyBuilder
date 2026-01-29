using System;
using System.Threading;
using Configs;
using Cysharp.Threading.Tasks;
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

        protected virtual void Awake()
        {
            Initialize();
        }

        protected virtual void OnDestroy()
        {
            canvasGroup.DOKill();
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

            _isOn = true;
            gameObject.SetActive(true);
            SetProgress(0f);
            canvasGroup.alpha = 1f;
        }

        public async UniTask ShowLoadingScreenAsync(CancellationToken token = default)
        {
            if (_isOn) 
                return;

            _isOn = true;
            gameObject.SetActive(true);
            SetProgress(0f);

            var tween = FadeLoadingScreen(0f, 1f);

            try
            {
                await tween.ToUniTask(TweenCancelBehaviour.CompleteAndCancelAwait, token);
            }
            catch (OperationCanceledException)
            {
                tween.Kill();
                throw;
            }
        }

        public async UniTask HideLoadingScreenAsync(CancellationToken token = default)
        {
            var tween = FadeLoadingScreen(1f, 0f, HideLoadingScreen);

            try
            {
                await tween.ToUniTask(TweenCancelBehaviour.CompleteAndCancelAwait, token);
            }
            catch (OperationCanceledException)
            {
                tween.Kill();
                throw;
            }
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