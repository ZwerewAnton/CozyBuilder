using Configs;
using PrimeTween;
using Settings;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

namespace Music
{
    [RequireComponent(typeof(AudioSource))]
    public class MusicPlayer : MonoBehaviour
    {
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private AudioClip menuMusicClip;
        [SerializeField] private AudioClip gameMusicClip;
        private AudioSource _audioSource;
        private ApplicationConfigs _configs;
        private Tween _fadeTween;

        private SettingsService _settingsService;
        private float _volume;

        private float Volume
        {
            get => _volume;
            set
            {
                _volume = value;
                SetMusicMixerVolume(_volume);
            }
        }

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            Volume = _settingsService.IsMusicOn ? _configs.audioOnValue : _configs.audioOffValue;
            _audioSource.loop = true;
            _audioSource.clip = menuMusicClip;
        }

        private void OnDestroy()
        {
            _settingsService.MusicChanged -= ApplyMusicState;
        }

        [Inject]
        private void Construct(SettingsService settingsService, ApplicationConfigs configs)
        {
            _configs = configs;
            _settingsService = settingsService;
            _settingsService.MusicChanged += ApplyMusicState;
        }

        public void Play(MusicType type)
        {
            switch (type)
            {
                case MusicType.MainMenu:
                    _audioSource.clip = menuMusicClip;
                    break;
                case MusicType.Level:
                    _audioSource.clip = gameMusicClip;
                    break;
                default:
                    _audioSource.Stop();
                    return;
            }

            if (_settingsService.IsMusicOn)
                FadeIn();
        }

        public void Stop()
        {
            FadeOut();
        }

        private void FadeIn()
        {
            _fadeTween.Stop();

            if (!_audioSource.isPlaying)
                _audioSource.Play();

            _fadeTween = Tween.Custom(
                Volume,
                _configs.audioOnValue,
                _configs.musicFadeTime,
                x => Volume = x
            ).OnComplete(() => _fadeTween = default);
        }

        private void FadeOut()
        {
            _fadeTween.Stop();

            _fadeTween = Tween.Custom(
                Volume,
                _configs.audioOffValue,
                _configs.musicFadeTime,
                x => Volume = x
            ).OnComplete(() =>
            {
                _audioSource.Stop();
                _fadeTween = default;
            });
        }

        private void ApplyMusicState(bool isOn)
        {
            if (isOn)
                FadeIn();
            else
                FadeOut();
        }

        private void SetMusicMixerVolume(float volume)
        {
            audioMixer.SetFloat(PropertiesStorage.MusicVolumeMixerProperty, volume);
        }
    }
}