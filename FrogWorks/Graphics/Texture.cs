using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FrogWorks
{
    public class Texture : IDisposable
    {
        public Texture2D XnaTexture { get; private set; }

        public Rectangle Bounds { get; private set; }

        public bool IsDisposed { get; private set; }

        public Texture(Texture2D xnaTexture)
            : this(xnaTexture, xnaTexture.Bounds)
        {
        }

        public Texture(Texture texture)
            : this(texture.XnaTexture, texture.Bounds)
        {
        }

        public Texture(Texture2D xnaTexture, Rectangle bounds)
        {
            if (xnaTexture == null)
                throw new NullReferenceException("XNA texture cannot be null.");

            XnaTexture = xnaTexture;
            Bounds = bounds.Intersect(xnaTexture.Bounds);
        }

        public Texture(Texture texture, Rectangle bounds)
            : this(texture.XnaTexture, bounds)
        {
        }

        public Texture(Texture2D xnaTexture, int x, int y, int width, int height)
            : this(xnaTexture, new Rectangle(x, y, width, height))
        {
        }

        public Texture(Texture texture, int x, int y, int width, int height)
            : this(texture.XnaTexture, new Rectangle(x, y, width, height))
        {
        }

        public void Draw(RendererBatch batch, Vector2 position, Vector2? origin = null, Vector2? scale = null, float angle = 0f, Color? color = null, SpriteEffects effects = SpriteEffects.None)
        {
            batch.Sprite.Draw(XnaTexture, position, Bounds, color ?? Color.White, angle, origin ?? Vector2.Zero, scale ?? Vector2.One, effects, 0f);
        }

        public Texture ClipRegion(Rectangle bounds)
        {
            return new Texture(XnaTexture, bounds);
        }

        public Texture ClipRegion(int x, int y, int width, int height)
        {
            return new Texture(XnaTexture, x, y, width, height);
        }

        public Texture Clone()
        {
            return new Texture(XnaTexture, Bounds);
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
                XnaTexture.Dispose();
                IsDisposed = true;
            }
        }
    }
}
