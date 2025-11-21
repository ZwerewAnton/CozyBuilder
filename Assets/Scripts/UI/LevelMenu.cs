using System;
using System.Threading.Tasks;
using Common;
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
        private LevelSaverMono _levelSaverMono;
        private SceneSwitcher _sceneSwitcher;
        private LevelMediator _levelMediator;
        private SfxPlayer _sfxPlayer;
        private GameState _gameState;
        private ParticlePlayer _particlePlayer;

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

            if (_gameState.IsLevelCompletedOnStart) 
                return;
            
            _particlePlayer.Play();
            PlayEndClip();
        }

        public void BackToMainMenu()
        {
            _levelSaverMono.SaveProgress();
            _ = LoadMainMenuScene();
        }
        
        private void PlayEndClip()
        {
            _sfxPlayer.PlayCompleteLevelClip();
        }
        
        private async Task LoadMainMenuScene()
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