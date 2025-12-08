using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "ApplicationConfigs", menuName = "My Assets/Configs/ApplicationConfigs")]
    public class ApplicationConfigs : ScriptableObject
    {
        [Header("Common")] 
        public int targetFrameRate = 120;
        
        [Header("Scene Loading")]
        public float minTimeToShowLoadingScreen = 1f;
        public float loadingScreenFadeTime = 0.5f;

        [Header("Camera"), SerializeField] 
        public CameraConfigs camera;
        
        [Header("Gameplay"), SerializeField] 
        public GameplayConfig gameplay;
        
        [Header("Audio")]
        public float audioOffValue = -80f;
        public float audioOnValue = -20f;
        public float musicFadeTime = 1f;
    }
}