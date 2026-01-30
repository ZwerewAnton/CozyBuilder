using System;
using Common;
using Cysharp.Threading.Tasks;
using Gameplay;
using Music;
using UnityEngine;
using Zenject;

namespace Level
{
    public class LevelSceneEntryPoint : MonoBehaviour
    {
        [SerializeField] private string editorLevelName;
        private GameState _gameState;
        private LevelService _levelService;
        private MusicPlayer _musicPlayer;

        private void Start()
        {
#if UNITY_EDITOR
            if (_gameState.SelectedLevelName == null && editorLevelName != "")
                _gameState.SelectedLevelName = editorLevelName;
#endif
            InitializeLevel().Forget();
            _musicPlayer.Play(MusicType.Level);
        }

        [Inject]
        private void Construct(LevelService levelService, MusicPlayer musicPlayer, GameState gameState)
        {
            _levelService = levelService;
            _musicPlayer = musicPlayer;
            _gameState = gameState;
        }

        private async UniTask InitializeLevel()
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