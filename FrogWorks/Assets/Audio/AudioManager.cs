using FMOD;
using System;
using System.IO;
using System.Runtime.InteropServices;
using FModSystem = FMOD.System;

namespace FrogWorks
{
    internal static partial class AudioManager
    {
        public static FModSystem System { get; private set; }

        public static ChannelGroup ChannelGroup { get; private set; }

        static bool IsActive { get; set; }

        public static void Initialize()
        {
            try
            {
                const int dspBufferLength = 256;
                const int dspBufferCount = 4;
                const int channelCount = 32;

                LoadNativeLibrary();

                FModSystem system;
                Factory.System_Create(out system);
                System = system;
                System.setDSPBufferSize(dspBufferLength, dspBufferCount);
                System.init(channelCount, INITFLAGS.CHANNEL_LOWPASS, (IntPtr)0);
                Audio.Initialize(channelCount);

                ChannelGroup channelGroup;
                System.getMasterChannelGroup(out channelGroup);
                ChannelGroup = channelGroup;

                IsActive = true;
            }
            catch
            {
                IsActive = false;
            }
        }

        public static void Update()
        {
            if (IsActive)
                System.update();
        }

        public static void Suspend()
        {
            if (IsActive)
                ChannelGroup.setPaused(true);
        }

        public static void Resume()
        {
            if (IsActive)
                ChannelGroup.setPaused(false);
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
                    var mode = MODE.OPENMEMORY | MODE.CREATESAMPLE;
                    var info = new CREATESOUNDEXINFO() { length = (uint)buffer.Length };
                    info.cbsize = Marshal.SizeOf(info);

                    Sound sound;
                    System.createSound(buffer, mode, ref info, out sound);
                    return new SoundEffect(filePath.CleanPath(), sound);
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
                    var mode = MODE.OPENMEMORY | MODE.CREATESTREAM;
                    var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                    var info = new CREATESOUNDEXINFO { length = (uint)buffer.Length };
                    info.cbsize = Marshal.SizeOf(info);

                    Sound sound;
                    System.createSound(buffer, mode, ref info, out sound);
                    return new SoundTrack(filePath.CleanPath(), sound, handle, buffer);
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
    }
}
