using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Infrastructure.SceneManagement
{
    public class SceneLoader
    {
        private bool _isLoading;
        public event Action<float> SceneLoadingUpdated;

        public async UniTask LoadSceneAsync(SceneType sceneType, CancellationToken token = default)
        {
            if (_isLoading)
                return;

            _isLoading = true;

            try
            {
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
                    await UniTask.Yield(token);

                    if (token.IsCancellationRequested)
                    {
                        operation.allowSceneActivation = true;
                        _isLoading = false;
                        return;
                    }
                }

                SceneLoadingUpdated?.Invoke(1f);

                operation.allowSceneActivation = true;
                while (!operation.isDone)
                {
                    await UniTask.Yield(token);
                    if (token.IsCancellationRequested)
                    {
                        _isLoading = false;
                        return;
                    }
                }
            }
            finally
            {
                _isLoading = false;
            }
        }
    }
}