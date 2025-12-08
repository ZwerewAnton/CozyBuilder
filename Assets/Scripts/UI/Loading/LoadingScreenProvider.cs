using Infrastructure.SceneManagement;
using UnityEngine;
using Zenject;

namespace UI.Loading
{
    public class LoadingScreenProvider : MonoBehaviour
    {
        [SerializeField] private LoadingScreen defaultScreen;
        [SerializeField] private BootstrapLoadingScreen bootstrapScreen;

        private SceneLocator _sceneLocator;

        [Inject]
        private void Construct(SceneLocator sceneLocator)
        {
            _sceneLocator = sceneLocator;
        }
        
        public LoadingScreenBase Get()
        {
            var sceneType = _sceneLocator.CurrentScene;
            return sceneType == SceneType.Boot
                ? bootstrapScreen
                : defaultScreen;
        }

        public LoadingScreenBase GetDefault()
        {
            return defaultScreen;
        }
    }
}