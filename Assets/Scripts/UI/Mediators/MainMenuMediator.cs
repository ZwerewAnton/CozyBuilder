using System.Collections.Generic;
using Music;
using UI.Common;
using UI.Common.Dialog;
using UI.MainMenu;
using UI.MainMenu.LevelScroll;
using UnityEngine;
using Zenject;

namespace UI.Mediators
{
    public class MainMenuMediator : MonoBehaviour
    {
        [SerializeField] private TapToPlayPanel tapToPlayPanel;
        [SerializeField] private SimplePanel menuPanel;
        [SerializeField] private SimplePanel miniHousePanel;
        [SerializeField] private MainMenu.MainMenu mainMenu;
        [SerializeField] private LevelsScrollController scrollController;
        [SerializeField] private ActionButton playButton;
        [SerializeField] private ActionButton deleteButton;
        [SerializeField] private CancelAcceptCloseDialog deleteDialog;
        [SerializeField] private SettingsMediator settingsMediator;

        private SfxPlayer _sfxPlayer;

        [Inject]
        private void Construct(SfxPlayer sfxPlayer)
        {
            _sfxPlayer = sfxPlayer;
        }

        private void OnEnable()
        {
            tapToPlayPanel.Clicked += mainMenu.FirstTap;
            tapToPlayPanel.Clicked += _sfxPlayer.PlayTapToPlayClip;
            playButton.Clicked += _sfxPlayer.PlayStartGameClip;
            playButton.Clicked += mainMenu.Play;
            deleteButton.Clicked += ShowDeleteDialog;
            deleteDialog.Completed += OnDeleteDialogCompleted;
        }

        private void OnDisable()
        {
            tapToPlayPanel.Clicked -= mainMenu.FirstTap;
            tapToPlayPanel.Clicked -= _sfxPlayer.PlayTapToPlayClip;
            playButton.Clicked -= _sfxPlayer.PlayStartGameClip;
            playButton.Clicked -= mainMenu.Play;
            deleteButton.Clicked -= ShowDeleteDialog;
            deleteDialog.Completed -= OnDeleteDialogCompleted;
        }

        public void InitializeLevelScroll(List<LevelItemModel> models)
        {
            scrollController.Initialize(models);
        }

        public void UpdateLevelScroll(List<LevelItemModel> models)
        {
            scrollController.UpdateModels(models);
        }

        public void MoveLevelScrollToItem(string levelName)
        {
            scrollController.MoveToItem(levelName);
        }

        public LevelItemModel GetScrollTargetItem()
        {
            return scrollController.GetTargetItemModel();
        }

        public void ShowTapToPlayPanel()
        {
            tapToPlayPanel.Show();
            miniHousePanel.Show();
            menuPanel.Hide();
        }
        
        public void HideTapToPlayPanel()
        {
            tapToPlayPanel.Hide();
            miniHousePanel.Hide();
            menuPanel.Show();
        }

        private void OnDeleteDialogCompleted(DialogResult result)
        {
            if (result == DialogResult.Accept)
                mainMenu.DeleteSaveData();
            _sfxPlayer.PlayDefaultButtonClip();
        }

        private void ShowDeleteDialog()
        {
            settingsMediator.ToggleMenu();
            deleteDialog.Show();
            _sfxPlayer.PlayDropdownMenuButtonClip();
        }
    }
}