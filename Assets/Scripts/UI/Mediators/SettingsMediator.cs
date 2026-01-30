using System;
using Music;
using Settings;
using UI.Common;
using UI.Settings;
using UnityEngine;
using Zenject;

namespace UI.Mediators
{
    public class SettingsMediator : MonoBehaviour
    {
        [SerializeField] private SettingsDropdownMenu dropdownMenu;
        [SerializeField] private ActionButton settingsButton;
        [SerializeField] private ToggleButton musicButton;
        [SerializeField] private ToggleButton soundButton;

        private SettingsService _settingsService;
        private SfxPlayer _sfxPlayer;

        private void Awake()
        {
            BindToggleButton(musicButton, ToggleMusic, UpdateMusicButtonState);
            BindToggleButton(soundButton, ToggleSound, UpdateSoundButtonState);
            settingsButton.Clicked += ToggleMenu;
            settingsButton.Clicked += PlayMenuButtonClip;
        }

        private void Start()
        {
            musicButton.Initialize();
            soundButton.Initialize();

            UpdateMusicButtonState();
            UpdateSoundButtonState();

            _settingsService.MusicChanged += musicButton.SetState;
            _settingsService.SoundChanged += soundButton.SetState;
        }

        private void OnDestroy()
        {
            UnbindToggleButton(musicButton, ToggleMusic, UpdateMusicButtonState);
            UnbindToggleButton(soundButton, ToggleSound, UpdateSoundButtonState);
            settingsButton.Clicked -= ToggleMenu;
            settingsButton.Clicked -= PlayMenuButtonClip;

            _settingsService.MusicChanged -= musicButton.SetState;
            _settingsService.SoundChanged -= soundButton.SetState;
        }

        [Inject]
        private void Construct(SettingsService settingsService, SfxPlayer sfxPlayer)
        {
            _settingsService = settingsService;
            _sfxPlayer = sfxPlayer;
        }

        public void ToggleMenu()
        {
            dropdownMenu.ToggleMenu();
        }

        private void BindToggleButton(ToggleButton button, Action toggleAction, Action updateAction)
        {
            button.Clicked += PlaySettingButtonClip;
            button.Clicked += toggleAction;
            button.Clicked += updateAction;
        }

        private void UnbindToggleButton(ToggleButton button, Action toggleAction, Action updateAction)
        {
            button.Clicked -= PlaySettingButtonClip;
            button.Clicked -= toggleAction;
            button.Clicked -= updateAction;
        }

        private void UpdateMusicButtonState()
        {
            musicButton.SetState(_settingsService.IsMusicOn);
        }

        private void UpdateSoundButtonState()
        {
            soundButton.SetState(_settingsService.IsSoundOn);
        }

        private void ToggleMusic()
        {
            _settingsService.ToggleMusic();
        }

        private void ToggleSound()
        {
            _settingsService.ToggleSound();
        }

        private void PlaySettingButtonClip()
        {
            _sfxPlayer.PlayDefaultButtonClip();
        }

        private void PlayMenuButtonClip()
        {
            _sfxPlayer.PlayDropdownMenuButtonClip();
        }
    }
}