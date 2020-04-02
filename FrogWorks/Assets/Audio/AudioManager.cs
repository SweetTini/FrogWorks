using System;
using System.IO;
using System.Runtime.InteropServices;

namespace FrogWorks
{
    using FmodSystem = FMOD.System;
    using FmodFactory = FMOD.Factory;
    using Sound = FMOD.Sound;
    using ChannelGroup = FMOD.ChannelGroup;
    using CreateSoundDexInfo = FMOD.CREATESOUNDEXINFO;
    using FmodInitFlags = FMOD.INITFLAGS;
    using FmodMode = FMOD.MODE;

    internal static partial class AudioManager
    {
        public static FmodSystem System { get; private set; }

        static bool IsActive { get; set; }

        public static void Initialize()
        {
            const int dspBufferLength = 256;
            const int dspBufferCount = 4;
            const int channelCount = 32;

            if (TryLoadNativeLiabrary())
            {
                FmodSystem system;
                FmodFactory.System_Create(out system);
                System = system;
                System.setDSPBufferSize(dspBufferLength, dspBufferCount);
                System.init(channelCount, FmodInitFlags.CHANNEL_LOWPASS, (IntPtr)0);
                IsActive = true;
            }
        }

        public static void Update()
        {
            if (IsActive)
                System.update();
        }

        public static void Dispose()
        {
            if (IsActive)
            {
                System.release();
                IsActive = false;
            }
        }

        public static ChannelGroup? CreateChannelGroup(string name)
        {
            if (IsActive)
            {
                ChannelGroup channelGroup;
                System.createChannelGroup(name, out channelGroup);
                return channelGroup;
            }

            return null;
        }

        public static SoundEffect LoadSoundEffect(string filePath)
        {
            if (IsActive)
            {
                var buffer = LoadFileAsBuffer(filePath, ".ogg");

                if (buffer != null)
                {
                    var mode = FmodMode.OPENMEMORY | FmodMode.CREATESAMPLE;
                    var info = new CreateSoundDexInfo();
                    info.length = (uint)buffer.Length;
                    info.cbsize = Marshal.SizeOf(info);

                    Sound sound;
                    System.createSound(buffer, mode, ref info, out sound);
                    return new SoundEffect(sound);
                }
            }

            return null;
        }

        public static SoundTrack LoadSoundTrack(string filePath)
        {
            if (IsActive)
            {
                var buffer = LoadFileAsBuffer(filePath, ".ogg");

                if (buffer != null)
                {
                    var mode = FmodMode.OPENMEMORY | FmodMode.CREATESTREAM;
                    var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                    var info = new CreateSoundDexInfo();
                    info.length = (uint)buffer.Length;
                    info.cbsize = Marshal.SizeOf(info);

                    Sound sound;
                    System.createSound(buffer, mode, ref info, out sound);
                    return new SoundTrack(sound, handle, buffer);
                }
            }

            return null;
        }

        static byte[] LoadFileAsBuffer(string filePath, params string[] fileTypes)
        {
            var stream = AssetManager.GetStream(filePath, fileTypes);

            if (stream != null)
            {
                using (stream)
                {
                    var buffer = new byte[16 * 1024];
                    byte[] result;

                    using (var memoryStream = new MemoryStream())
                    {
                        int bytesRead;
                        while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                            memoryStream.Write(buffer, 0, bytesRead);
                        result = memoryStream.ToArray();
                    }

                    return result;
                }
            }

            return null;
        }

        static bool TryLoadNativeLiabrary()
        {
            try
            {
                LoadNativeLibrary();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
