using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace FrogWorks
{
    public sealed class Texture
    {
        private Texture2D XnaTexture { get; set; }

        public Rectangle Bounds { get; private set; }

        public Point Size { get; private set; }

        public int Width => Size.X;

        public int Height => Size.Y;

        public Vector2 MinUV { get; private set; }

        public float LeftUV => MinUV.X;

        public float TopUV => MaxUV.Y;

        public Vector2 MaxUV { get; private set; }

        public float RightUV => MaxUV.X;

        public float BottomUV => MaxUV.Y;

        public Texture(Texture texture)
            : this(texture.XnaTexture, texture.Bounds)
        {
        }

        public Texture(Texture texture, Rectangle bounds)
            : this(texture.XnaTexture, bounds)
        {
        }

        public Texture(Texture texture, Point location, Point size)
            : this(texture.XnaTexture, new Rectangle(location, size))
        {
        }

        public Texture(Texture texture, int x, int y, int width, int height)
            : this(texture.XnaTexture, new Rectangle(x, y, width, height))
        {
        }

        internal Texture(Texture2D xnaTexture, Rectangle bounds)
        {
            if (xnaTexture == null)
                throw new NullReferenceException("XNA texture cannot be null.");

            XnaTexture = xnaTexture;
            Bounds = bounds.Intersect(xnaTexture.Bounds);
            Size = bounds.Size;

            var size = XnaTexture.Bounds.Size.ToVector2();
            MinUV = Bounds.Location.ToVector2().Divide(size);
            MaxUV = (Bounds.Location + Bounds.Size).ToVector2().Divide(size);
        }        

        internal Texture(Texture2D xnaTexture, Point location, Point size)
            : this(xnaTexture, new Rectangle(location, size))
        {
        }

        internal Texture(Texture2D xnaTexture, int x, int y, int width, int height)
            : this(xnaTexture, new Rectangle(x, y, width, height))
        {
        }

        public void Draw(RendererBatch batch, Vector2 position, Vector2 origin, Vector2 scale, 
                         float angle, Color color, SpriteEffects effects)
        {
            batch.DrawSprites((sprite) => 
            {
                sprite.Draw(
                    XnaTexture, position.Round(), Bounds, 
                    color, angle, origin, scale, effects, 0f);
            });
        }

        public void Draw(RendererBatch batch, Vector2 position, Rectangle bounds, Vector2 origin, 
                         Vector2 scale, float angle, Color color, SpriteEffects effects)
        {
            batch.DrawSprites((sprite) => 
            {
                sprite.Draw(
                    XnaTexture, position.Round(), bounds, 
                    color, angle, origin, scale, effects, 0f);
            }, true);
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

        #region Static Methods
        public static Texture Load(string fileName)
        {
            var xnaTexture = AssetManager.GetFromCache(fileName, FromStream);
            return xnaTexture != null ? new Texture(xnaTexture, xnaTexture.Bounds) : null;
        }

        public static Texture[] Split(Texture2D xnaTexture, Point frameSize)
        {
            frameSize = frameSize.Abs();

            var columns = xnaTexture.Width / frameSize.X;
            var rows = xnaTexture.Height / frameSize.Y;
            var frames = new Texture[columns * rows];

            for (int i = 0; i < frames.Length; i++)
            {
                var position = new Point(i % columns, i / columns) * frameSize;
                frames[i] = new Texture(xnaTexture, position, frameSize);
            }

            return frames;
        }

        public static Texture[] Split(Texture2D xnaTexture, int frameWidth, int frameHeight)
        {
            return Split(xnaTexture, new Point(frameWidth, frameHeight));
        }

        public static Texture[] Split(Texture texture, Point frameSize)
        {
            return Split(texture.XnaTexture, frameSize);
        }

        public static Texture[] Split(Texture texture, int frameWidth, int frameHeight)
        {
            return Split(texture.XnaTexture, new Point(frameWidth, frameHeight));
        }

        internal static Texture2D FromStream(string path)
        {
            var stream = AssetManager.GetStream(path, ".png");

            if (stream != null)
            {
                using (stream)
                {
                    var graphicsDevice = Runner.Application.Game.GraphicsDevice;
                    return Texture2D.FromStream(graphicsDevice, stream);
                }
            }

            return null;
        }
        #endregion
    }
}
