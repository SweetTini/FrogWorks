using System;

namespace FrogWorks
{
    using Sound = FMOD.Sound;
    using Channel = FMOD.Channel;
    using TimeUnit = FMOD.TIMEUNIT;

    public abstract class SoundClip : IDisposable
    {
        protected Sound Sound { get; private set; }

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
            var system = AudioManager.System;
            var channelGroup = AudioManager.ChannelGroup;

            Channel channel;
            system.playSound(Sound, channelGroup, false, out channel);
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
