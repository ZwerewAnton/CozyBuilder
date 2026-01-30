using System;
using System.Collections.Generic;
using Common;
using Cysharp.Threading.Tasks;
using Infrastructure.SceneManagement;
using Level;
using Music;
using SaveSystem;
using UI.MainMenu.LevelScroll;
using UI.Mediators;
using UnityEngine;
using Zenject;

namespace UI.MainMenu
{
    public class MainMenu : MonoBehaviour
    {
        private GameState _gameState;
        private MainMenuMediator _mainMenuMediator;
        private MusicPlayer _musicPlayer;
        private ILevelsRepository _repository;
        private ISaveLoadService _saveLoadService;
        private SceneSwitcher _sceneSwitcher;

        private void Start()
        {
            SetPanelsVisibility();
            SetScrollControllerContent();
        }

        [Inject]
        private void Construct(
            MainMenuMediator mainMenuMediator,
            GameState gameState,
            MusicPlayer musicPlayer,
            ILevelsRepository repository,
            ISaveLoadService saveLoadService,
            SceneSwitcher sceneSwitcher)
        {
            _mainMenuMediator = mainMenuMediator;
            _gameState = gameState;
            _musicPlayer = musicPlayer;
            _repository = repository;
            _saveLoadService = saveLoadService;
            _sceneSwitcher = sceneSwitcher;
        }

        public void FirstTap()
        {
            _mainMenuMediator.HideTapToPlayPanel();
            _musicPlayer.Play(MusicType.MainMenu);
        }

        public void Play()
        {
            var targetModel = _mainMenuMediator.GetScrollTargetItem();
            _gameState.SelectedLevelName = targetModel.levelName;
            LoadLevelScene().Forget();
        }

        public void DeleteSaveData()
        {
            _saveLoadService.DeleteSaveDirectory();
            UpdateScrollControllerContent();
        }

        private async UniTask LoadLevelScene()
        {
            try
            {
                await _sceneSwitcher.LoadSceneAsync(SceneType.Level);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load level: {ex}");
            }
        }

        private void SetPanelsVisibility()
        {
            if (_gameState.IsFirstMenuLaunch)
            {
                _mainMenuMediator.ShowTapToPlayPanel();
                _gameState.MarkMenuAsLaunched();
            }
            else
            {
                _mainMenuMediator.HideTapToPlayPanel();
                _musicPlayer.Play(MusicType.MainMenu);
            }
        }

        private void SetScrollControllerContent()
        {
            var items = CreateScrollItems();
            _mainMenuMediator.InitializeLevelScroll(items);
            _mainMenuMediator.MoveLevelScrollToItem(_gameState.SelectedLevelName);
        }

        private void UpdateScrollControllerContent()
        {
            var items = CreateScrollItems();
            _mainMenuMediator.UpdateLevelScroll(items);
        }

        private List<LevelItemModel> CreateScrollItems()
        {
            var items = new List<LevelItemModel>();
            var saveData = _saveLoadService.ProgressData.progressLevelsSaveData;

            foreach (var levelData in _repository.Levels)
            {
                var progress = saveData.Find(data => data.levelName == levelData.LevelName);
                items.Add(new LevelItemModel
                {
                    levelIcon = levelData.Icon,
                    levelName = levelData.LevelName,
                    progressPercent = progress?.progress ?? 0
                });
            }

            return items;
        }
    }
}