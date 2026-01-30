using System;
using Common;
using Cysharp.Threading.Tasks;
using Gameplay;
using Infrastructure.SceneManagement;
using Music;
using UI.Mediators;
using UnityEngine;
using VFX;
using Zenject;

namespace UI
{
    public class LevelMenu : MonoBehaviour
    {
        private GameState _gameState;
        private LevelMediator _levelMediator;
        private LevelSaverMono _levelSaverMono;
        private ParticlePlayer _particlePlayer;
        private SceneSwitcher _sceneSwitcher;
        private SfxPlayer _sfxPlayer;

        [Inject]
        private void Construct(
            LevelSaverMono levelSaverMono,
            SceneSwitcher sceneSwitcher,
            LevelMediator levelMediator,
            SfxPlayer sfxPlayer,
            GameState gameState,
            ParticlePlayer particlePlayer)
        {
            _levelSaverMono = levelSaverMono;
            _sceneSwitcher = sceneSwitcher;
            _levelMediator = levelMediator;
            _sfxPlayer = sfxPlayer;
            _gameState = gameState;
            _particlePlayer = particlePlayer;
        }

        public void ShowEndScreen()
        {
            _levelMediator.ShowHomeButton();
            _levelMediator.HideDetailsScroll();
            _levelMediator.ResetCamera();

            if (_gameState.IsLevelCompletedOnStart)
                return;

            _particlePlayer.Play();
            PlayEndClip();
        }

        public void BackToMainMenu()
        {
            _levelSaverMono.SaveProgress();
            LoadMainMenuScene().Forget();
        }

        private void PlayEndClip()
        {
            _sfxPlayer.PlayCompleteLevelClip();
        }

        private async UniTask LoadMainMenuScene()
        {
            try
            {
                await _sceneSwitcher.LoadSceneAsync(SceneType.MainMenu);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load main menu scene: {ex}");
            }
        }
    }
}