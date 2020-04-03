using System.Runtime.InteropServices;

namespace FrogWorks
{
    using Sound = FMOD.Sound;

    public sealed class SoundTrack : SoundClip
    {
        GCHandle _bufferHandle;
        byte[] _buffer;

        internal SoundTrack(string name, Sound sound, GCHandle bufferHandle, byte[] buffer)
            : base(name, sound)
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
