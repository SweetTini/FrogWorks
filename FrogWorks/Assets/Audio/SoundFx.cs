using ChaiFoxes.FMODAudio;

namespace FrogWorks
{
    public class SoundFx : Audio
    {
        protected SoundFx(Sound sound) 
            : base(sound)
        {
        }

        #region Static Methods
        public static SoundFx Load(string filePath)
        {
            Sound sound;

            return TryGetFromCache(filePath, false, out sound)
                ? new SoundFx(sound)
                : null;
        }
        #endregion
    }
}
