using ChaiFoxes.FMODAudio;

namespace FrogWorks
{
    public class SoundEffect : Audio
    {
        protected SoundEffect(Sound sound) 
            : base(sound)
        {
        }

        #region Static Methods
        public static SoundEffect Load(string filePath)
        {
            Sound sound;

            return TryGetFromCache(filePath, false, out sound)
                ? new SoundEffect(sound)
                : null;
        }
        #endregion
    }
}
