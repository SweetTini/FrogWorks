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
            var sound = AssetManager.GetFromCache(filePath, LoadSound);
            return sound != null ? new SoundEffect(sound) : null;
        }
        #endregion
    }
}
