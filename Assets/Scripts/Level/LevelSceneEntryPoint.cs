using System;
using System.Threading.Tasks;
using Common;
using Gameplay;
using Music;
using UnityEngine;
using Zenject;

namespace Level
{
    public class LevelSceneEntryPoint : MonoBehaviour
    {
        [SerializeField] private string editorLevelName;
        private LevelService _levelService;
        private MusicPlayer _musicPlayer;
        private GameState _gameState;
        
        [Inject]
        private void Construct(LevelService levelService, MusicPlayer musicPlayer, GameState gameState)
        {
            _levelService = levelService;
            _musicPlayer = musicPlayer;
            _gameState = gameState;
        }

        private void Start()
        {
#if UNITY_EDITOR
            if (_gameState.SelectedLevelName == null && editorLevelName != "")
            {
                _gameState.SelectedLevelName = editorLevelName;
            }
#endif
            _ = InitializeLevel();
            _musicPlayer.Play(MusicType.Level);
        }

        private async Task InitializeLevel()
        {
            try
            {
                await _levelService.InitializeLevel();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to initialize level: {ex}");
            }
        }
    }
}