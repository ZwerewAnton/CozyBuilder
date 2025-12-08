namespace Settings
{
    public static class PropertiesStorage
    {
        public const float AudioOffValue = -80f;
        public const float AudioOnValue = -20f;
        public const string MusicPlayerPrefsKey = "MusicVolume";
        public const string SoundPlayerPrefsKey = "SoundVolume";
        public const string MusicVolumeMixerProperty = "MusicVolumeParam";
        public const string SoundVolumeMixerProperty = "SoundVolumeParam";

        public static string GetMusicKey()
        {
            return MusicPlayerPrefsKey;
        }
        
        public static string GetSoundKey()
        {
            return SoundPlayerPrefsKey;
        }
        
        public static string GetMusicVolumeMixerParameter()
        {
            return MusicVolumeMixerProperty;
        }
        
        public static string GetSoundVolumeMixerParameter()
        {
            return SoundVolumeMixerProperty;
        }
    }
}