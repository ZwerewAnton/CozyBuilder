using System;
using System.Threading;
using Configs;
using Cysharp.Threading.Tasks;
using Infrastructure.SceneManagement;
using SaveSystem;
using UI.Mediators;
using UnityEngine;
using Zenject;

namespace Infrastructure
{
    public class Bootstrap : MonoBehaviour
    {
        private CancellationTokenSource _cts;
        private LoadingScreenMediator _loadingScreenMediator;
        private ISaveLoadService _saveSystemService;
        private SceneSwitcher _sceneSwitcher;

        private void Awake()
        {
            _loadingScreenMediator.ShowLoadingScreenImmediately();
        }

        private void Start()
        {
            InitializeServices().Forget();
        }

        private void OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }

        public event Action LoadingCompleted;

        [Inject]
        private void Construct(
            ISaveLoadService saveSystemService,
            ApplicationConfigs configs,
            LoadingScreenMediator loadingScreenMediator,
            SceneSwitcher sceneSwitcher)
        {
            SetTargetFrameRate(configs.targetFrameRate);
            _saveSystemService = saveSystemService;
            _loadingScreenMediator = loadingScreenMediator;
            _sceneSwitcher = sceneSwitcher;
        }

        private async UniTask InitializeServices()
        {
            try
            {
                _cts = new CancellationTokenSource();
                await _saveSystemService.LoadProgressDataAsync(_cts.Token);
                LoadingCompleted?.Invoke();
#if !UNITY_EDITOR
                await CompleteInitializationAsync();
#endif
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                Debug.LogError($"Initialization failed: {ex}");
            }
        }

        private async UniTask CompleteInitializationAsync()
        {
            await _sceneSwitcher.LoadSceneAsync(SceneType.MainMenu);
        }

        private static void SetTargetFrameRate(int targetFrameRate)
        {
            Application.targetFrameRate = 120;
        }
    }
}