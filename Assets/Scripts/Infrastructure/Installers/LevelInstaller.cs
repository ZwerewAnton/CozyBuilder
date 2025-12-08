using System;
using _1_LEVEL_REWORK.New.Instances;
using Cameras;
using Cameras.Input;
using Cameras.Movement;
using Gameplay;
using Gameplay.Movement;
using Gameplay.Movement.Input;
using Input.TouchRegistry;
using UI;
using UI.Mediators;
using UI.Raycast;
using UnityEngine;
using VFX;
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
        [SerializeField] private CanvasRaycasterHandler canvasRaycasterHandler;
        [SerializeField] private ParticlePlayer particlePlayer;
        
        public override void InstallBindings()
        {
            BindCanvasRaycasterHandler();
            BindUIRaycasterHelper();
            BindCameraHandler();
            BindCameraInputProvider();
            BindOrbitCameraMovement();
            BindDetailPrefabSpawner();
            BindTouchRegistry();
            BindDetailViewMoverInputProvider();
            BindDetailViewMover();
            BindLevelState();
            BindLevelService();
            BindLevelMediator();
            BindLevelInteractableCoordinator();
            BindLevelSaverMono();
            BindLevelMenu();
            BindParticlePlayer();
        }

        private void BindParticlePlayer()
        {
            Container.Bind<ParticlePlayer>().FromInstance(particlePlayer).AsSingle().NonLazy();
        }

        private void BindCanvasRaycasterHandler()
        {
            Container.Bind<CanvasRaycasterHandler>().FromInstance(canvasRaycasterHandler).AsSingle().NonLazy();
        }

        private void BindUIRaycasterHelper()
        {
            Container.Bind<UIRaycasterHelper>().AsSingle().NonLazy();
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

        private void BindDetailViewMoverInputProvider()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            Container
                .Bind(typeof(IDetailViewMoverInputProvider), typeof(IDisposable))
                .To<MouseDetailViewMoverInputProvider>()
                .AsSingle()
                .NonLazy();
#elif UNITY_ANDROID || UNITY_IOS
            Container
                .Bind(typeof(IDetailViewMoverInputProvider), typeof(IDisposable))
                .To<TouchDetailViewMoverInputProvider>()
                .AsSingle()
                .NonLazy();
#else
            Container
                .Bind(typeof(IDetailViewMoverInputProvider), typeof(IDisposable))
                .To<MouseDetailViewMoverInputProvider>()
                .AsSingle()
                .NonLazy();
#endif
        }

        private void BindTouchRegistry()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            Container
                .Bind(typeof(ITouchPointerLock))
                .To<DummyTouchPointerLock>()
                .AsSingle()
                .NonLazy();
#elif UNITY_ANDROID || UNITY_IOS
            Container
                .Bind(typeof(TouchRegistry), typeof(ITouchPointerLock), typeof(IDisposable))
                .To<TouchRegistry>()
                .AsSingle()
                .NonLazy();
#else
            Container
                .Bind(typeof(ITouchPointerLock))
                .To<DummyTouchPointerLock>()
                .AsSingle()
                .NonLazy();
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