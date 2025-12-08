using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Infrastructure.SceneManagement
{
    public class SceneLoader
    {
        public event Action<float> SceneLoadingUpdated;
        
        private bool _isLoading;
        
        public async Task LoadSceneAsync(SceneType sceneType, CancellationToken token = default)
        {
            if (_isLoading)
                return;

            _isLoading = true;

            var operation = SceneManager.LoadSceneAsync(sceneType.ToString());
            if (operation == null)
            {
                _isLoading = false;
                return;
            }

            operation.allowSceneActivation = false;
            while (operation.progress < 0.9f)
            {
                var progress = Mathf.Clamp01(operation.progress / 0.9f);
                SceneLoadingUpdated?.Invoke(progress);
                await Task.Yield();
                
                if (token.IsCancellationRequested)
                {
                    _isLoading = false;
                    return;
                }
            }
            SceneLoadingUpdated?.Invoke(1f);

            operation.allowSceneActivation = true;
            while (!operation.isDone)
            {
                await Task.Yield();
                if (token.IsCancellationRequested)
                {
                    _isLoading = false;
                    return;
                }
            }

            _isLoading = false;
        }
    }
}