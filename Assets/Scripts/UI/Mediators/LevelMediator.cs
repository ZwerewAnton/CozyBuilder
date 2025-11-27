using System;
using System.Collections.Generic;
using Cameras.Movement;
using Gameplay;
using UI.Common;
using UI.Game.DetailsScroll;
using UI.MainMenu;
using UnityEngine;
using Zenject;

namespace UI.Mediators
{
    public class LevelMediator : MonoBehaviour
    {
        [SerializeField] private DetailsScrollController detailsScrollController;
        [SerializeField] private ActionButton backButton;
        [SerializeField] private ActionButton homeButton;
        [SerializeField] private ResetCameraButton cameraResetButton;

        private LevelMenu _levelMenu;
        private LevelService _levelService;
        private OrbitCameraMovement _orbitCameraMovement;

        [Inject]
        private void Construct(
            LevelMenu levelMenu, 
            LevelService levelService, 
            OrbitCameraMovement orbitCameraMovement
        )
        {
            _levelMenu = levelMenu;
            _levelService = levelService;
            _orbitCameraMovement = orbitCameraMovement;
        }

        private void OnEnable()
        {
            backButton.Clicked += OnBackButton;
            homeButton.Clicked += OnHomeButton;
            cameraResetButton.Clicked += OnResetCameraButton;
            _orbitCameraMovement.CameraMoved += OnCameraMoved;
            _levelService.LevelCompleted += OnLevelCompleted;
        }

        private void OnDisable()
        {
            backButton.Clicked -= OnBackButton;
            homeButton.Clicked -= OnHomeButton;
            cameraResetButton.Clicked -= OnResetCameraButton;
            _orbitCameraMovement.CameraMoved -= OnCameraMoved;
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

        private void OnResetCameraButton()
        {
            _orbitCameraMovement.ResetCamera();
        }

        private void OnCameraMoved()
        {
            cameraResetButton.SetButtonVisibility(
                _orbitCameraMovement.DesiredHeightOffset,
                _orbitCameraMovement.DesiredZoomOffset
            );
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