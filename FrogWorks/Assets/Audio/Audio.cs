using ChaiFoxes.FMODAudio;
using System;
using System.Collections.Generic;
using System.IO;

namespace FrogWorks
{
    public abstract class Audio
    {
        private float _volume = 1f;

        private static Dictionary<string, Sound> SoundCache { get; } = new Dictionary<string, Sound>();

        private static Dictionary<string, Sound> StreamCache { get; } = new Dictionary<string, Sound>();

        public static float MasterVolume { get; set; } = 1f;

        protected Sound Sound { get; private set; }

        protected SoundChannel Channel { get; private set; }

        public float Volume
        {
            get { return _volume; }
            set
            {
                _volume = value.Clamp(0f, 1f);

                if (Channel != null)
                    Channel.Volume = _volume * MasterVolume.Clamp(0f, 1f);
            }
        }

        public bool IsPlaying => Channel?.IsPlaying ?? false;

        public bool IsLooping => Channel?.Looping ?? false;

        protected Audio(Sound sound)
            : base()
        {
            if (sound == null)
                throw new NullReferenceException("FMOD sound cannot be null.");

            Sound = sound;
        }

        public void Play()
        {
            Channel = Sound.Play();
            Channel.Volume = _volume * MasterVolume.Clamp(0f, 1f);
        }

        public void Play(int loopCount)
        {
            Play();
            Channel.Loops = (loopCount.Abs() - 1).Max(0);
        }

        public void Loop()
        {
            Play();
            Channel.Looping = true;
        }

        public void Stop() => Channel?.Stop();

        public void Resume() => Channel?.Resume();

        public void Pause() => Channel?.Pause();

        #region Static Methods
        internal static Sound TryGetFromCache(string filePath, bool isStreamed)
        {
            Sound sound;

            var cache = isStreamed ? StreamCache : SoundCache;

            if (!cache.TryGetValue(filePath, out sound))
            {
                var contentDirectory = Runner.Application.Game.Content.RootDirectory;
                var absolutePath = Path.Combine(contentDirectory, filePath);

                try
                {
                    sound = isStreamed
                        ? AudioMgr.LoadStreamedSound(absolutePath)
                        : AudioMgr.LoadSound(absolutePath);

                    cache.Add(filePath, sound);
                }
                catch
                {
                    sound = null;
                }
            }

            return sound;
        }

        internal static void Dispose()
        {
            SoundCache.Clear();
            StreamCache.Clear();
        }
        #endregion
    }
}
