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
            Sound sound;

            return TryGetFromCache(filePath, true, out sound)
                ? new MusicTrack(sound)
                : null;
        }
        #endregion
    }
}
