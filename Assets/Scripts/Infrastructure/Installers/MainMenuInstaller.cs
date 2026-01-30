using UI.Mediators;
using UnityEngine;
using Zenject;

namespace Infrastructure.Installers
{
    public class MainMenuInstaller : MonoInstaller
    {
        [SerializeField] private SettingsMediator settingsMediator;
        [SerializeField] private MainMenuMediator mainMenuMediator;

        public override void InstallBindings()
        {
            BindMainMenuMediator();
            BindSettingsMediator();
        }

        private void BindMainMenuMediator()
        {
            Container.Bind<MainMenuMediator>().FromInstance(mainMenuMediator).AsSingle().NonLazy();
        }

        private void BindSettingsMediator()
        {
            Container.Bind<SettingsMediator>().FromInstance(settingsMediator).AsSingle().NonLazy();
        }
    }
}