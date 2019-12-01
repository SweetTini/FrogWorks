using ChaiFoxes.FMODAudio;

namespace FrogWorks
{
    public class MusicTrack : Audio
    {
        public int Position
        {
            get { return (int?)Channel?.TrackPosition ?? 0; }
            set
            {
                if (Channel != null)
                    Channel.TrackPosition = (uint)value;
            }
        }

        protected MusicTrack(Sound sound) 
            : base(sound)
        {
        }

        #region Static Methods
        public static MusicTrack Load(string filePath)
        {
            var sound = TryGetFromCache(filePath, true);
            return new MusicTrack(sound);
        }
        #endregion
    }
}
