using System;
using System.IO;
using System.Runtime.InteropServices;

namespace FrogWorks
{
    internal static partial class AudioManager
    {
        [DllImport("kernel32.dll", EntryPoint = "LoadLibrary")]
        static extern IntPtr LoadWinLibrary(string filePath);

        [DllImport("libdl.so.2", EntryPoint = "dlopen")]
        static extern IntPtr LoadUnixLibrary(string filename, int flags);

        static void LoadNativeLibrary()
        {
            var directory = Environment.Is64BitProcess ? "x64" : "x86";
            string fullPath;

            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Unix:
                    fullPath = Path.GetFullPath($"{directory}\\libfmod.so");
                    LoadUnixLibrary(fullPath, 0x001);
                    break;
                default:
                    fullPath = Path.GetFullPath($"{directory}\\fmod.dll");
                    LoadWinLibrary(fullPath);
                    break;
            }
        }
    }
}
