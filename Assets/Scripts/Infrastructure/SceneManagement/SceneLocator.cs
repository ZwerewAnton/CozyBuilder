using UnityEngine.SceneManagement;

namespace Infrastructure.SceneManagement
{
    public class SceneLocator
    {
        public SceneType CurrentScene { get; private set; } = SceneType.Boot;

        public void UpdateCurrentScene()
        {
            var name = SceneManager.GetActiveScene().name;
            
            CurrentScene = name switch
            {
                "Boot" => SceneType.Boot,
                "MainMenu"  => SceneType.MainMenu,
                "Level"     => SceneType.Level,
                _ => SceneType.MainMenu
            };
        }
    }
}