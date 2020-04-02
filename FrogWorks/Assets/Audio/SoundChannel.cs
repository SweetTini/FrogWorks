namespace FrogWorks
{
    using Channel = FMOD.Channel;
    using Mode = FMOD.MODE;
    using TimeUnit = FMOD.TIMEUNIT;

    public sealed class SoundChannel
    {
        Channel _channel;

        public SoundClip Clip { get; private set; }

        public int Index
        {
            get
            {
                int index;
                _channel.getIndex(out index);
                return index;
            }
        }

        public bool Loop
        {
            get { return MaxLoops != 0; }
            set { MaxLoops = value ? -1 : 0; }
        }

        public int MaxLoops
        {
            get 
            {
                int loopCount;
                _channel.getLoopCount(out loopCount);
                return loopCount;
            }
            set
            {
                value = value.Max(-1);

                var mode = value != 0
                    ? Mode.LOOP_NORMAL
                    : Mode.LOOP_OFF;

                _channel.setMode(mode);
                _channel.setLoopCount(value);
            }
        }

        public float Volume
        {
            get
            {
                float volume;
                _channel.getVolume(out volume);
                return volume;
            }
            set
            {
                value = value.Clamp(0f, 1f);
                _channel.setVolume(value);
            }
        }

        public float Pitch
        {
            get
            {
                float pitch;
                _channel.getPitch(out pitch);
                return pitch;
            }
            set
            {
                _channel.setPitch(value);
            }
        }

        public float LowPass
        {
            get
            {
                float gain;
                _channel.getLowPassGain(out gain);
                return gain;
            }
            set
            {
                value = value.Clamp(0f, 1f);
                _channel.setLowPassGain(value);
            }
        }

        public int Position
        {
            get
            {
                uint position;
                _channel.getPosition(out position, TimeUnit.MS);
                return (int)position;
            }
            set
            {
                value = value.Mod(Clip.Length);
                _channel.setPosition((uint)value, TimeUnit.MS);
            }
        }

        public bool IsPlaying
        {
            get 
            {
                bool isPlaying;
                _channel.isPlaying(out isPlaying);
                return isPlaying;
            }
        }

        internal SoundChannel(SoundClip clip, Channel channel)
            : base()
        {
            Clip = clip;
            _channel = channel;
        }

        public void Pause()
        {
            _channel.setPaused(true);
        }

        public void Resume()
        {
            _channel.setPaused(false);
        }

        public void Stop()
        {
            _channel.stop();
        }
    }
}
