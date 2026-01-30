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
        [SerializeField] private AudioClip dropdownMenuButtonClip;
        [SerializeField] private AudioClip defaultButtonClip;
        [SerializeField] private AudioClip installDetailClip;
        [SerializeField] private AudioClip completeLevelClip;
        private AudioSource _audioSource;
        private ApplicationConfigs _configs;

        private SettingsService _settingsService;

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

        [Inject]
        private void Construct(SettingsService settingsService, ApplicationConfigs configs)
        {
            _configs = configs;
            _settingsService = settingsService;
            _settingsService.SoundChanged += ApplySoundState;
        }

        public void PlayTapToPlayClip()
        {
            _audioSource.PlayOneShot(tapToPlayClip);
        }

        public void PlayStartGameClip()
        {
            _audioSource.PlayOneShot(playClip);
        }

        public void PlayDropdownMenuButtonClip()
        {
            _audioSource.PlayOneShot(dropdownMenuButtonClip);
        }

        public void PlayDefaultButtonClip()
        {
            _audioSource.PlayOneShot(defaultButtonClip);
        }

        public void PlayCompleteLevelClip()
        {
            _audioSource.PlayOneShot(completeLevelClip);
        }

        public void PlayInstallDetailClip()
        {
            _audioSource.PlayOneShot(installDetailClip);
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