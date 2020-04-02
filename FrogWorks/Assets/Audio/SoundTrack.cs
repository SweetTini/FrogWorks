using System.Runtime.InteropServices;

namespace FrogWorks
{
    using Sound = FMOD.Sound;

    public sealed class SoundTrack : Audio
    {
        GCHandle _bufferHandle;
        byte[] _buffer;

        internal SoundTrack(Sound sound, GCHandle bufferHandle, byte[] buffer)
            : base(sound)
        {
            _bufferHandle = bufferHandle;
            _buffer = buffer;
        }

        protected override void OnDispose()
        {
            if (_buffer != null)
                _bufferHandle.Free();
        }

        #region Static Methods
        public static SoundTrack Load(string filePath)
        {
            return AssetManager.GetFromCache(filePath, AudioManager.LoadSoundTrack);
        }
        #endregion
    }
}
