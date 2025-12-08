using Common;
using Configs;
using Infrastructure.SceneManagement;
using Input;
using Level;
using Music;
using SaveSystem;
using Settings;
using UI.Loading;
using UI.Mediators;
using UnityEngine;
using Zenject;

namespace Infrastructure.Installers
{
    public class BootstrapInstaller : MonoInstaller
    {
        [SerializeField] private ApplicationConfigs applicationConfigs;
        [SerializeField] private MusicPlayer musicPlayer;
        [SerializeField] private SfxPlayer sfxPlayer;
        [SerializeField] private LoadingScreenProvider loadingScreenProvider;
        [SerializeField] private LevelsRepository levelsRepository;
        
        public override void InstallBindings()
        {
            BindSceneLocator();
            BindApplicationConfigs();
            BindSceneLoader();
            BindSceneSwitcher();
            BindSaveLoadService();
            BindSettingsService();
            BindMusicPlayer();
            BindSfxPlayer();
            BindGameState();
            BindInputHandler();
            BindLevelsRepository();
            BindLoadingScreenMediator();
            BindLoadingScreenProvider();
        }

        private void BindSceneLocator()
        {
            Container.Bind<SceneLocator>().AsSingle().NonLazy();
        }

        private void BindLoadingScreenProvider()
        {
            Container.Bind<LoadingScreenProvider>().FromComponentInNewPrefab(loadingScreenProvider).AsSingle().NonLazy();
        }

        private void BindApplicationConfigs()
        {
            Container.Bind<ApplicationConfigs>().FromInstance(applicationConfigs).AsSingle().NonLazy();
        }
        
        private void BindSceneLoader()
        {
            Container.Bind<SceneLoader>().AsSingle().WhenInjectedInto<SceneSwitcher>().NonLazy();
        }
        
        private void BindSceneSwitcher()
        {
            Container.BindInterfacesAndSelfTo<SceneSwitcher>().AsSingle();
        }

        private void BindSaveLoadService()
        {
            Container.Bind<ISaveLoadService>().To<SaveLoadService>().AsSingle().NonLazy();
        }

        private void BindSettingsService()
        {
            Container.Bind<SettingsService>().AsSingle().NonLazy();
        }

        private void BindMusicPlayer()
        {
            Container.Bind<MusicPlayer>().FromComponentInNewPrefab(musicPlayer).AsSingle().NonLazy();
        }

        private void BindSfxPlayer()
        {
            Container.Bind<SfxPlayer>().FromComponentInNewPrefab(sfxPlayer).AsSingle().NonLazy();
        }

        private void BindLoadingScreenMediator()
        {
            Container.BindInterfacesAndSelfTo<LoadingScreenMediator>().AsSingle().NonLazy();
        }

        private void BindGameState()
        {
            Container.Bind<GameState>().AsSingle().NonLazy();
        }

        private void BindInputHandler()
        {
            Container.BindInterfacesAndSelfTo<InputHandler>().AsSingle().NonLazy();
        }
        
        private void BindLevelsRepository()
        {
            Container.Bind<ILevelsRepository>().FromInstance(levelsRepository).AsSingle().NonLazy();
        }
    }
}