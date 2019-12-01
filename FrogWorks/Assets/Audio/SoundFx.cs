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
            var sound = TryGetFromCache(filePath, false);
            return new SoundFx(sound);
        }
        #endregion
    }
}
