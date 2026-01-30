using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Zenject;

namespace Infrastructure.SceneManagement
{
    public class SceneSwitcher : IDisposable
    {
        private CancellationTokenSource _cts;
        private bool _isTransitioning;
        private ILoadingView _loadingView;
        private SceneLoader _sceneLoader;
        private SceneLocator _sceneLocator;

        public void Dispose()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }

        public event Action<float> SceneLoadingUpdated
        {
            add => _sceneLoader.SceneLoadingUpdated += value;
            remove => _sceneLoader.SceneLoadingUpdated -= value;
        }

        [Inject]
        private void Construct(ILoadingView loadingScreen, SceneLoader sceneLoader, SceneLocator sceneLocator)
        {
            _loadingView = loadingScreen;
            _sceneLoader = sceneLoader;
            _sceneLocator = sceneLocator;
        }

        public async UniTask LoadSceneAsync(SceneType sceneType)
        {
            if (_isTransitioning)
                return;

            _isTransitioning = true;
            _cts = new CancellationTokenSource();

            try
            {
                await _loadingView.ShowAsync(_cts.Token);
                await _sceneLoader.LoadSceneAsync(sceneType, _cts.Token);
                await _loadingView.HideAsync(_cts.Token);
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                _isTransitioning = false;
                _sceneLocator.UpdateCurrentScene();
            }
        }
    }
}