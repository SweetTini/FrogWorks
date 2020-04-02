using System;
using System.IO;
using System.Runtime.InteropServices;

namespace FrogWorks
{
    using FModSystem = FMOD.System;
    using FModFactory = FMOD.Factory;
    using Sound = FMOD.Sound;
    using SoundExInfo = FMOD.CREATESOUNDEXINFO;
    using InitFlags = FMOD.INITFLAGS;
    using Mode = FMOD.MODE;

    internal static partial class AudioManager
    {
        public static FModSystem System { get; private set; }

        static bool IsActive { get; set; }

        public static void Initialize()
        {
            const int dspBufferLength = 256;
            const int dspBufferCount = 4;
            const int channelCount = 32;

            if (TryLoadNativeLibrary())
            {
                FModSystem system;
                FModFactory.System_Create(out system);
                System = system;
                System.setDSPBufferSize(dspBufferLength, dspBufferCount);
                System.init(channelCount, InitFlags.CHANNEL_LOWPASS, (IntPtr)0);

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

        public static SoundEffect LoadSoundEffect(string filePath)
        {
            if (IsActive)
            {
                var buffer = LoadBuffer(filePath, ".ogg");

                if (buffer != null)
                {
                    var mode = Mode.OPENMEMORY | Mode.CREATESAMPLE;
                    var info = new SoundExInfo() { length = (uint)buffer.Length };
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
                var buffer = LoadBuffer(filePath, ".ogg");

                if (buffer != null)
                {
                    var mode = Mode.OPENMEMORY | Mode.CREATESTREAM;
                    var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                    var info = new SoundExInfo { length = (uint)buffer.Length };
                    info.cbsize = Marshal.SizeOf(info);

                    Sound sound;
                    System.createSound(buffer, mode, ref info, out sound);
                    return new SoundTrack(sound, handle, buffer);
                }
            }

            return null;
        }

        static byte[] LoadBuffer(string filePath, params string[] fileTypes)
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

        static bool TryLoadNativeLibrary()
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
