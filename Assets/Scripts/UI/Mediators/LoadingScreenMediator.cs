using System;
using System.Threading.Tasks;
using Infrastructure.SceneManagement;
using UI.Loading;
using Zenject;

namespace UI.Mediators
{
    public class LoadingScreenMediator : ILoadingView, IInitializable, IDisposable
    {
        private LoadingScreenProvider _loadingScreenProvider;
        private SceneSwitcher _sceneSwitcher;
        
        [Inject]
        private void Construct(SceneSwitcher sceneSwitcher, LoadingScreenProvider loadingScreenProvider)
        {
            _sceneSwitcher = sceneSwitcher;
            _loadingScreenProvider = loadingScreenProvider;
        }

        public void Initialize()
        {
            _sceneSwitcher.SceneLoadingUpdated += _loadingScreenProvider.GetDefault().SetProgress;
        }

        public void Dispose()
        {
            _sceneSwitcher.SceneLoadingUpdated -= _loadingScreenProvider.GetDefault().SetProgress;
        }
        
        public void ShowLoadingScreenImmediately()
        {
            _loadingScreenProvider.Get().ShowLoadingScreenImmediately();
        }

        public async Task ShowAsync()
        {
            await _loadingScreenProvider.Get().ShowLoadingScreenAsync();
        }

        public async Task HideAsync()
        {
            await _loadingScreenProvider.Get().HideLoadingScreenAsync();
        }
    }
}