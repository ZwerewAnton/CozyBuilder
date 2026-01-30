using System.Threading;
using Configs;
using Cysharp.Threading.Tasks;
using PrimeTween;
using UnityEngine;
using Zenject;

namespace UI.Loading
{
    public abstract class LoadingScreenBase : MonoBehaviour
    {
        [SerializeField] protected CanvasGroup canvasGroup;

        private ApplicationConfigs _configs;
        private bool _isOn;

        protected virtual void Awake()
        {
            Initialize();
        }

        [Inject]
        private void Construct(ApplicationConfigs configs)
        {
            _configs = configs;
        }

        protected virtual void Initialize()
        {
            SetProgress(0f);
        }

        public virtual void SetProgress(float value)
        {
        }

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

            await FadeLoadingScreen(0f, 1f, token);
        }

        public async UniTask HideLoadingScreenAsync(CancellationToken token = default)
        {
            await FadeLoadingScreen(1f, 0f, token);
            HideLoadingScreen();
        }

        protected virtual void HideLoadingScreen()
        {
            gameObject.SetActive(false);
            _isOn = false;
        }

        protected virtual UniTask FadeLoadingScreen(float startValue, float targetValue,
            CancellationToken token = default)
        {
            canvasGroup.alpha = startValue;
            var duration = _configs.loadingScreenFadeTime;

            return Tween.Alpha(canvasGroup, targetValue, duration, Ease.Linear)
                .ToYieldInstruction()
                .WithCancellation(token);
        }
    }
}