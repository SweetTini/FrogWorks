﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

namespace FrogWorks
{
    public class Texture : IDisposable
    {
        static Dictionary<string, Texture2D> Cache { get; } = new Dictionary<string, Texture2D>();

        public Texture2D XnaTexture { get; private set; }

        public Rectangle Bounds { get; private set; }

        public Point Size { get; private set; }

        public int Width => Size.X;

        public int Height => Size.Y;

        public Vector2 UpperUV => Bounds.Location.ToVector2().Divide(XnaTexture.Bounds.Size.ToVector2());

        public Vector2 LowerUV => (Bounds.Location + Bounds.Size).ToVector2().Divide(XnaTexture.Bounds.Size.ToVector2());

        public float LeftUV => UpperUV.X;

        public float TopUV => LowerUV.Y;

        public float RightUV => LowerUV.X;

        public float BottomUV => LowerUV.Y;

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
            Size = bounds.Size;
        }

        public Texture(Texture texture, Rectangle bounds)
            : this(texture.XnaTexture, bounds)
        {
        }

        public Texture(Texture2D xnaTexture, Point location, Point size)
            : this(xnaTexture, new Rectangle(location, size))
        {
        }

        public Texture(Texture texture, Point location, Point size)
            : this(texture.XnaTexture, new Rectangle(location, size))
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

        public void Draw(RendererBatch batch, Vector2 position, Vector2 origin, Vector2 scale, float angle, Color color, SpriteEffects effects)
        {
            batch.DrawSprites((sprite) => sprite.Draw(XnaTexture, position, Bounds, color, angle, origin, scale, effects, 0f));
        }

        public void Draw(RendererBatch batch, Vector2 position, Rectangle bounds, Vector2 origin, Vector2 scale, float angle, Color color, SpriteEffects effects)
        {
            batch.DrawSprites((sprite) => sprite.Draw(XnaTexture, position, bounds, color, angle, origin, scale, effects, 0f), true);
        }

        public Texture ClipRegion(Rectangle bounds)
        {
            return new Texture(XnaTexture, bounds);
        }

        public Texture ClipRegion(Point location, Point size)
        {
            return new Texture(XnaTexture, location, size);
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

        #region Static Methods
        public static Texture Load(string filePath)
        {
            var xnaTexture = TryGetFromCache(filePath);
            return new Texture(xnaTexture, xnaTexture.Bounds);
        }

        public static Texture[] Split(Texture2D xnaTexture, int frameWidth, int frameHeight)
        {
            var columns = xnaTexture.Width / frameWidth;
            var rows = xnaTexture.Height / frameHeight;
            var frames = new Texture[columns * rows];

            for (int i = 0; i < frames.Length; i++)
            {
                var x = i % columns;
                var y = i / columns;

                frames[i] = new Texture(xnaTexture, x * frameWidth, y * frameHeight, frameWidth, frameHeight);
            }

            return frames;
        }

        public static Texture[] Split(Texture texture, int frameWidth, int frameHeight)
        {
            return Split(texture.XnaTexture, frameWidth, frameHeight);
        }

        internal static Texture2D TryGetFromCache(string filePath)
        {
            Texture2D xnaTexture;

            if (!Cache.TryGetValue(filePath, out xnaTexture))
            {
                var absolutePath = Path.Combine(Runner.Application.ContentDirectory, filePath);

                using (var stream = File.OpenRead(absolutePath))
                {
                    xnaTexture = Texture2D.FromStream(Runner.Application.Game.GraphicsDevice, stream);
                    Cache.Add(filePath, xnaTexture);
                }
            }

            return xnaTexture;
        }

        internal static void DisposeCache()
        {
            foreach (var xnaTexture in Cache.Values)
                xnaTexture.Dispose();

            Cache.Clear();
        }
        #endregion
    }
}
