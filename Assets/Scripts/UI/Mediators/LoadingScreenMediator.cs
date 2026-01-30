using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Infrastructure.SceneManagement;
using UI.Loading;
using Zenject;

namespace UI.Mediators
{
    public class LoadingScreenMediator : ILoadingView, IInitializable, IDisposable
    {
        private LoadingScreenProvider _loadingScreenProvider;
        private SceneSwitcher _sceneSwitcher;

        public void Dispose()
        {
            _sceneSwitcher.SceneLoadingUpdated -= _loadingScreenProvider.GetDefault().SetProgress;
        }

        public void Initialize()
        {
            _sceneSwitcher.SceneLoadingUpdated += _loadingScreenProvider.GetDefault().SetProgress;
        }

        public async UniTask ShowAsync(CancellationToken token = default)
        {
            await _loadingScreenProvider.Get().ShowLoadingScreenAsync(token);
        }

        public async UniTask HideAsync(CancellationToken token = default)
        {
            await _loadingScreenProvider.Get().HideLoadingScreenAsync(token);
        }

        [Inject]
        private void Construct(SceneSwitcher sceneSwitcher, LoadingScreenProvider loadingScreenProvider)
        {
            _sceneSwitcher = sceneSwitcher;
            _loadingScreenProvider = loadingScreenProvider;
        }

        public void ShowLoadingScreenImmediately()
        {
            _loadingScreenProvider.Get().ShowLoadingScreenImmediately();
        }
    }
}