using System;

namespace FrogWorks
{
    using Sound = FMOD.Sound;
    using Channel = FMOD.Channel;
    using TimeUnit = FMOD.TIMEUNIT;

    public abstract class SoundClip : IDisposable
    {
        int _maxLoops;
        float _volume = 1f;
        float _lowPass = 1f;

        protected Sound Sound { get; private set; }

        public bool Loop
        {
            get { return MaxLoops < 0; }
            set { MaxLoops = value ? -1 : 0; }
        }

        public int MaxLoops
        {
            get { return _maxLoops; }
            set { _maxLoops = value.Max(-1); }
        }

        public float Volume
        {
            get { return _volume; }
            set { _volume = value.Clamp(0f, 1f); }
        }

        public float Pitch { get; set; } = 1f;

        public float LowPass
        {
            get { return _lowPass; }
            set { _lowPass = value.Clamp(0f, 1f); }
        }

        public int Length
        {
            get
            {
                uint length;
                Sound.getLength(out length, TimeUnit.MS);
                return (int)length;
            }
        }

        public bool IsDisposed { get; private set; }

        protected SoundClip(Sound sound)
            : base()
        {
            Sound = sound;
        }

        public SoundChannel Play()
        {
            Channel channel;
            AudioManager.System.playSound(Sound, default, false, out channel);
            return new SoundChannel(this, channel);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        void Dispose(bool isDisposing)
        {
            if (isDisposing && !IsDisposed)
            {
                Sound.release();
                OnDispose();
                IsDisposed = true;
            }
        }

        protected virtual void OnDispose()
        {
        }
    }
}
