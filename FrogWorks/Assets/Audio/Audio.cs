using System;

namespace FrogWorks
{
    using Sound = FMOD.Sound;

    public abstract class Audio : IDisposable
    {
        protected Sound Sound { get; private set; }

        public bool IsDisposed { get; private set; }

        protected Audio(Sound sound)
            : base()
        {
            Sound = sound;
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
