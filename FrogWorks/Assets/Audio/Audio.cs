using ChaiFoxes.FMODAudio;
using System;
using System.IO;

namespace FrogWorks
{
    public abstract class Audio
    {
        private float _volume = 1f;

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
        internal static Sound LoadSound(string filePath)
        {
            var fullPath = AssetManager.GetFullPath(filePath, ".ogg");

            if (!string.IsNullOrEmpty(fullPath))
            {
                var extension = Path.GetExtension(fullPath);
                var rootDirectory = Runner.Application.Game.Content.RootDirectory;
                fullPath = Path.Combine(rootDirectory, Path.ChangeExtension(filePath, extension));
                return AudioMgr.LoadSound(fullPath);
            }

            return null;
        }

        internal static Sound LoadStreamedSound(string filePath)
        {
            var fullPath = AssetManager.GetFullPath(filePath, ".ogg");

            if (!string.IsNullOrEmpty(fullPath))
            {
                var extension = Path.GetExtension(fullPath);
                var rootDirectory = Runner.Application.Game.Content.RootDirectory;
                fullPath = Path.Combine(rootDirectory, Path.ChangeExtension(filePath, extension));
                return AudioMgr.LoadStreamedSound(fullPath);
            }

            return null;
        }
        #endregion
    }
}
