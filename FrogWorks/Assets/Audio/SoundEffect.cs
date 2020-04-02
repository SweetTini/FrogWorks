namespace FrogWorks
{
    using Sound = FMOD.Sound;

    public sealed class SoundEffect : Audio
    {
        internal SoundEffect(Sound sound)
            : base(sound)
        {
        }

        #region Static Methods
        public static SoundEffect Load(string filePath)
        {
            return AssetManager.GetFromCache(filePath, AudioManager.LoadSoundEffect);
        }
        #endregion
    }
}
