using _1_LEVEL_REWORK.New.Instances;
using Cameras;
using Cameras.Input;
using Cameras.Movement;
using Gameplay;
using Gameplay.Movement;
using UI;
using UI.Mediators;
using UnityEngine;
using Zenject;

namespace Infrastructure.Installers
{
    public class LevelInstaller : MonoInstaller
    {
        [SerializeField] private DetailPrefabSpawner detailPrefabSpawner;
        [SerializeField] private LevelMediator levelMediator;
        [SerializeField] private DetailViewMover detailViewMover;
        [SerializeField] private CameraHandler cameraHandler;
        [SerializeField] private LevelSaverMono levelSaverMono;
        [SerializeField] private LevelMenu levelMenu;
        
        public override void InstallBindings()
        {
            BindCameraHandler();
            BindCameraInputProvider();
            BindOrbitCameraMovement();
            BindDetailPrefabSpawner();
            BindDetailViewMoverInput();
            BindDetailViewMover();
            BindLevelState();
            BindLevelService();
            BindLevelMediator();
            BindLevelInteractableCoordinator();
            BindLevelSaverMono();
            BindLevelMenu();
        }

        private void BindLevelMenu()
        {
            Container.Bind<LevelMenu>().FromInstance(levelMenu).AsSingle().NonLazy();
        }

        private void BindLevelSaverMono()
        {
            Container.Bind<LevelSaverMono>().FromInstance(levelSaverMono).AsSingle().NonLazy();
        }

        private void BindCameraHandler()
        {
            Container.Bind<CameraHandler>().FromInstance(cameraHandler).AsSingle().NonLazy();
        }

        private void BindCameraInputProvider()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            Container.Bind<ICameraInputProvider>().To<MouseCameraInputProvider>().AsSingle();
#elif UNITY_ANDROID || UNITY_IOS
            Container.Bind<ICameraInputProvider>().To<TouchCameraInputProvider>().AsSingle();
#else
            Container.Bind<ICameraInputProvider>().To<MouseCameraInputProvider>().AsSingle();
#endif
        }
        
        private void BindOrbitCameraMovement()
        {
            Container.Bind<OrbitCameraMovement>().AsSingle().NonLazy();
        }
        
        private void BindDetailPrefabSpawner()
        {
            Container.Bind<DetailPrefabSpawner>().FromInstance(detailPrefabSpawner).AsSingle().NonLazy();
        }

        private void BindDetailViewMoverInput()
        {
            Container.Bind<IDetailViewMoverInputProvider>().To<MouseDetailViewMoverInputProvider>().AsSingle();
        }
        
        private void BindDetailViewMover()
        {
            Container.Bind<DetailViewMover>().FromInstance(detailViewMover).AsSingle().NonLazy();
        }
        
        private void BindLevelState()
        {
            Container.Bind<LevelState>().AsSingle().NonLazy();
        }
        
        private void BindLevelService()
        {
            Container.BindInterfacesAndSelfTo<LevelService>().AsSingle();
        }

        private void BindLevelMediator()
        {
            Container.Bind<LevelMediator>().FromInstance(levelMediator).AsSingle().NonLazy();
        }

        private void BindLevelInteractableCoordinator()
        {
            Container.Bind<LevelInteractableCoordinator>().AsSingle().NonLazy();
        }
    }
}