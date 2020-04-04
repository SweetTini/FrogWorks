using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FrogWorks
{
    public static class Audio
    {
        static SoundChannel[] _channels;
        static float _volume = 1f;

        public static ReadOnlyCollection<SoundChannel> Channels { get; private set; }

        public static float Volume
        {
            get { return _volume; }
            set { _volume = value.Clamp(0f, 1f); }
        }

        internal static void Initialize(int maxChannels)
        {
            _channels = new SoundChannel[maxChannels];
            Channels = new ReadOnlyCollection<SoundChannel>(_channels);
        }

        public static void Unload()
        {
            ClearAllChannels();
            AssetManager.ClearCache<SoundEffect>();
            AssetManager.ClearCache<SoundTrack>();
        }

        public static SoundChannel Play<T>(
            string filePath,
            float volume = 1f,
            float pitch = 1f,
            float lowPass = 1f)
            where T : SoundClip
        {
            var clip = LoadClip<T>(filePath);
            return ConfigureClip(clip, 1, volume, pitch, lowPass);
        }

        public static SoundChannel Loop<T>(
            string filePath,
            float volume = 1f,
            float pitch = 1f,
            float lowPass = 1f)
            where T : SoundClip
        {
            return Loop<T>(filePath, 0, volume, pitch, lowPass);
        }

        public static SoundChannel Loop<T>(
            string filePath,
            int loopCount,
            float volume = 1f,
            float pitch = 1f,
            float lowPass = 1f)
            where T : SoundClip
        {
            var clip = LoadClip<T>(filePath);
            return ConfigureClip(clip, loopCount, volume, pitch, lowPass);
        }

        public static void Pause<T>(string filePath)
            where T : SoundClip
        {
            ToChannelList<T>(filePath).ForEach(channel => channel.Pause());
        }

        public static void Resume<T>(string filePath)
            where T : SoundClip
        {
            ToChannelList<T>(filePath).ForEach(channel => channel.Resume());
        }

        public static void Stop<T>(string filePath)
            where T : SoundClip
        {
            ToChannelList<T>(filePath).ForEach(channel => channel.Stop());
        }

        public static void PauseAll<T>()
            where T : SoundClip
        {
            ToChannelList<T>().ForEach(channel => channel.Pause());
        }

        public static void PauseAll()
        {
            ToChannelList().ForEach(channel => channel.Pause());
        }

        public static void ResumeAll<T>()
            where T : SoundClip
        {
            ToChannelList<T>().ForEach(channel => channel.Resume());
        }

        public static void ResumeAll()
        {
            ToChannelList().ForEach(channel => channel.Resume());
        }

        public static void StopAll<T>()
            where T : SoundClip
        {
            ToChannelList<T>().ForEach(channel => channel.Stop());
        }

        public static void StopAll()
        {
            ToChannelList().ForEach(channel => channel.Stop());
        }

        static SoundClip LoadClip<T>(string filePath)
            where T : SoundClip
        {
            var clip = null as SoundClip;
            var type = typeof(T);

            if (type == typeof(SoundEffect)) clip = SoundEffect.Load(filePath);
            else if (type == typeof(SoundTrack)) clip = SoundTrack.Load(filePath);

            return clip;
        }

        static SoundChannel ConfigureClip(
            SoundClip clip,
            int maxLoops,
            float volume,
            float pitch,
            float lowPass)
        {
            if (clip != null)
            {
                var channel = clip.Play(true);
                AssignChannel(channel);

                channel.MaxLoops = maxLoops;
                channel.Volume = volume;
                channel.Volume *= _volume;
                channel.Pitch = pitch;
                channel.LowPass = lowPass;
                channel.Resume();

                return channel;
            }

            return null;
        }

        static void AssignChannel(SoundChannel channel)
        {
            var index = _channels.Length - channel.Index - 1;
            _channels[index]?.Stop();
            _channels[index] = channel;
        }

        static void ClearAllChannels()
        {
            if (_channels != null)
            {
                for (int i = 0; i < _channels.Length; i++)
                {
                    _channels[i]?.Stop();
                    _channels[i] = null;
                }
            }
        }

        static List<SoundChannel> ToChannelList<T>()
            where T : SoundClip
        {
            var channels = Channels?
                .Where(channel => channel != null
                    && channel.Clip is T)
                .ToList();

            return channels ?? new List<SoundChannel>();
        }

        static List<SoundChannel> ToChannelList<T>(string filePath)
            where T : SoundClip
        {
            filePath = filePath.CleanPath();

            var channels = Channels?
                .Where(channel => channel != null
                    && channel.Clip is T
                    && channel.Clip.Name == filePath)
                .ToList();

            return channels ?? new List<SoundChannel>();
        }

        static List<SoundChannel> ToChannelList()
        {
            var channels = Channels?
                .Where(channel => channel != null)
                .ToList();

            return channels ?? new List<SoundChannel>();
        }
    }
}
