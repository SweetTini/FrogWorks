﻿using FMOD;

namespace FrogWorks
{
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
            get { return MaxLoops < 1; }
            set { MaxLoops = value ? 0 : 1; }
        }

        public int MaxLoops
        {
            get 
            {
                int loopCount;
                _channel.getLoopCount(out loopCount);
                return loopCount + 1;
            }
            set
            {
                value = value.Abs();

                var mode = value != 1
                    ? MODE.LOOP_NORMAL
                    : MODE.LOOP_OFF;

                _channel.setMode(mode);
                _channel.setLoopCount(value - 1);
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
                float lowPass;
                _channel.getLowPassGain(out lowPass);
                return lowPass;
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
                _channel.getPosition(out position, TIMEUNIT.MS);
                return (int)position;
            }
            set
            {
                value = value.Mod(Clip.Length);
                _channel.setPosition((uint)value, TIMEUNIT.MS);
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
            _channel = channel;

            Clip = clip;
            MaxLoops = Clip.MaxLoops;
            Volume = Clip.Volume;
            Pitch = Clip.Pitch;
            LowPass = Clip.LowPass;
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
