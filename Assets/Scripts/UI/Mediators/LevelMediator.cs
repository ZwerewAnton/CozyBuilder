using System;
using System.Collections.Generic;
using Gameplay;
using UI.Common;
using UI.Game.DetailsScroll;
using UnityEngine;
using Zenject;

namespace UI.Mediators
{
    public class LevelMediator : MonoBehaviour
    {
        [SerializeField] private DetailsScrollController detailsScrollController;
        [SerializeField] private ActionButton backButton;
        [SerializeField] private ActionButton homeButton;

        private LevelMenu _levelMenu;
        private LevelService _levelService;

        [Inject]
        private void Construct(LevelMenu levelMenu, LevelService levelService)
        {
            _levelMenu = levelMenu;
            _levelService = levelService;
        }

        private void OnEnable()
        {
            backButton.Clicked += OnBackButton;
            homeButton.Clicked += OnHomeButton;
            _levelService.LevelCompleted += OnLevelCompleted;
        }

        private void OnDisable()
        {
            backButton.Clicked -= OnBackButton;
            homeButton.Clicked -= OnHomeButton;
            _levelService.LevelCompleted -= OnLevelCompleted;
        }

        public event Action<DragOutInfo> DetailItemDragOutStarted
        {
            add => detailsScrollController.DragOutStarted += value;
            remove => detailsScrollController.DragOutStarted -= value;
        }
        
        public void InitializeDetailsScroll(List<DetailItemModel> models)
        {
            detailsScrollController.Initialize(models);
        }        
        
        public void MarkItemDragOutState(string detailId, bool isDragOut)
        {
            detailsScrollController.MarkItemDragOutState(detailId, isDragOut);
        }
        
        public void UpdateScrollController(List<DetailItemModel> models)
        {
            detailsScrollController.UpdateModels(models);
        }

        public void ShowHomeButton()
        {
            homeButton.gameObject.SetActive(true);
        }

        public void HideDetailsScroll()
        {
            detailsScrollController.gameObject.SetActive(false);
        }

        private void OnHomeButton()
        {
            BackToMainMenu();
        }

        private void OnBackButton()
        {
            BackToMainMenu();
        }

        private void BackToMainMenu()
        {
            _levelMenu.BackToMainMenu();
        }

        private void OnLevelCompleted()
        {
            _levelMenu.ShowEndScreen();
        }
    }
}