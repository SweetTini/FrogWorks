using FMOD;

namespace FrogWorks
{
    public sealed class SoundEffect : SoundClip
    {
        internal SoundEffect(string name, Sound sound)
            : base(name, sound)
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
