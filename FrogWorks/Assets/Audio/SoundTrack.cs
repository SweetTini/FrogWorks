﻿using ChaiFoxes.FMODAudio;

namespace FrogWorks
{
    public class SoundTrack : Audio
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

        protected SoundTrack(Sound sound)
            : base(sound)
        {
        }

        #region Static Methods
        public static SoundTrack Load(string filePath)
        {
            var sound = AssetManager.GetFromCache(filePath, LoadStreamedSound);
            return sound != null ? new SoundTrack(sound) : null;
        }
        #endregion
    }
}