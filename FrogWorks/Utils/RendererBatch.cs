using Microsoft.Xna.Framework.Graphics;
using System;

namespace FrogWorks
{
    public class RendererBatch : IDisposable
    {
        public SpriteBatch Sprite { get; private set; }

        public PrimitiveBatch Primitive { get; private set; }

        public bool IsDisposed { get; private set; }

        internal RendererBatch(GraphicsDevice graphicsDevice)
        {
            Sprite = new SpriteBatch(graphicsDevice);
            Primitive = new PrimitiveBatch(graphicsDevice);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (isDisposing && !IsDisposed)
            {
                Sprite.Dispose();
                Primitive.Dispose();
                IsDisposed = true;
            }
        }
    }
}
