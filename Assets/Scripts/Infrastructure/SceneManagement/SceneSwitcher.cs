using System;
using System.Threading;
using System.Threading.Tasks;
using UI.Mediators;
using UnityEngine;
using Zenject;

namespace Infrastructure.SceneManagement
{
    public class SceneSwitcher : IDisposable
    {
        private CancellationTokenSource _cts;
        private ILoadingView _loadingView;
        private SceneLoader _sceneLoader;
        private SceneLocator _sceneLocator;
        private bool _isTransitioning;
        
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

        public async Task LoadSceneAsync(SceneType sceneType)
        {
            if (_isTransitioning) 
                return;
            _isTransitioning = true;
            
            _cts = new CancellationTokenSource();

            try
            {
                await _loadingView.ShowAsync();
                await _sceneLoader.LoadSceneAsync(sceneType, _cts.Token);
                await _loadingView.HideAsync();
            }
            finally
            {
                _isTransitioning = false;
                _sceneLocator.UpdateCurrentScene();
            }
        }

        public void Dispose()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }
    }
}