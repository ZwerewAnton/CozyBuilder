using System;
using UnityEngine;
using Utils;

namespace Settings
{
    public class SettingsService
    {
        private bool _isMusicOn, _isSoundOn;

        public SettingsService()
        {
            _isMusicOn = LoadVolumeState(PropertiesStorage.MusicPlayerPrefsKey);
            _isSoundOn = LoadVolumeState(PropertiesStorage.SoundPlayerPrefsKey);
        }

        public bool IsMusicOn => _isMusicOn;
        public bool IsSoundOn => _isSoundOn;

        public event Action<bool> MusicChanged;
        public event Action<bool> SoundChanged;

        public void ToggleMusic()
        {
            ToggleAudio(PropertiesStorage.MusicPlayerPrefsKey, ref _isMusicOn);
            MusicChanged?.Invoke(_isMusicOn);
        }

        public void ToggleSound()
        {
            ToggleAudio(PropertiesStorage.SoundPlayerPrefsKey, ref _isSoundOn);
            SoundChanged?.Invoke(_isSoundOn);
        }

        private static void ToggleAudio(string prefsKey, ref bool param)
        {
            param = !param;
            SetVolume(prefsKey, param);
        }

        private static void SetVolume(string key, bool value)
        {
            PlayerPrefsUtils.SetBool(key, value);
            PlayerPrefs.Save();
        }

        private static bool LoadVolumeState(string key)
        {
            return PlayerPrefsUtils.GetBool(key);
        }
    }
}