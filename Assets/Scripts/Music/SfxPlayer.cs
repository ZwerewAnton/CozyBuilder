using Configs;
using Settings;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

namespace Music
{
    [RequireComponent(typeof(AudioSource))]
    public class SfxPlayer : MonoBehaviour
    {
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private AudioClip tapToPlayClip;
        [SerializeField] private AudioClip playClip;
        [SerializeField] private AudioClip menuButtonClip;
        [SerializeField] private AudioClip settingButtonClip;
        [SerializeField] private AudioClip completeLevelClip;
        
        private SettingsService _settingsService;
        private ApplicationConfigs _configs;
        private AudioSource _audioSource;

        [Inject]
        private void Construct(SettingsService settingsService, ApplicationConfigs configs)
        {
            _configs = configs;
            _settingsService = settingsService;
            _settingsService.SoundChanged += ApplySoundState;
        }
        
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            ApplySoundState(_settingsService.IsSoundOn);
        }

        private void OnDestroy()
        {
            _settingsService.SoundChanged -= ApplySoundState;
        }

        public void PlayTapToPlayClip()
        {
            _audioSource.PlayOneShot(tapToPlayClip);
        }
        
        public void PlayStartGameClip()
        {
            _audioSource.PlayOneShot(playClip);
        }
        
        public void PlayMenuButtonClip()
        {
            _audioSource.PlayOneShot(menuButtonClip);
        }
        
        public void PlaySettingButtonClip()
        {
            _audioSource.PlayOneShot(settingButtonClip);
        }
        
        public void PlayCompleteLevelClip()
        {
            _audioSource.PlayOneShot(completeLevelClip);
        }

        private void ApplySoundState(bool isOn)
        {
            SetMusicMixerVolume(isOn ? _configs.audioOnValue : _configs.audioOffValue);
        }
        
        private void SetMusicMixerVolume(float volume)
        {
            audioMixer.SetFloat(PropertiesStorage.SoundVolumeMixerProperty, volume);
        }
    }
}