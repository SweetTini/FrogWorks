using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FrogWorks
{
    public class BackgroundImage : Component
    {
        private Vector2 _position;

        public Texture Texture { get; protected set; }

        public Rectangle Bounds => new Rectangle(0, 0, Texture.Width, Texture.Height);

        public Rectangle DrawableRegion { get; private set; }

        public Vector2 Position
        {
            get { return _position; }
            set
            {
                if (value == _position) return;
                _position = value;
                if (ParentLayer?.Camera != null)
                    UpdateDrawableRegion(ParentLayer.Camera);
            }
        }

        public float X
        {
            get { return _position.X; }
            set
            {
                if (value == _position.X) return;
                _position.X = value;
                if (ParentLayer?.Camera != null)
                    UpdateDrawableRegion(ParentLayer.Camera);
            }
        }

        public float Y
        {
            get { return _position.Y; }
            set
            {
                if (value == _position.Y) return;
                _position.Y = value;
                if (ParentLayer?.Camera != null)
                    UpdateDrawableRegion(ParentLayer.Camera);
            }
        }

        public Vector2 DrawPosition
        {
            get { return Position + (Parent?.Position ?? Vector2.Zero); }
            set { Position = value - (Parent?.Position ?? Vector2.Zero); }
        }

        public Color Color { get; set; } = Color.White;

        public float Opacity { get; set; } = 1f;

        public SpriteEffects SpriteEffects { get; set; }

        public bool FlipHorizontally
        {
            get { return (SpriteEffects & SpriteEffects.FlipHorizontally) == SpriteEffects.FlipHorizontally; }
            set
            {
                SpriteEffects = value
                    ? (SpriteEffects | SpriteEffects.FlipHorizontally)
                    : (SpriteEffects & ~SpriteEffects.FlipHorizontally);
            }
        }

        public bool FlipVertically
        {
            get { return (SpriteEffects & SpriteEffects.FlipVertically) == SpriteEffects.FlipVertically; }
            set
            {
                SpriteEffects = value
                    ? (SpriteEffects | SpriteEffects.FlipVertically)
                    : (SpriteEffects & ~SpriteEffects.FlipVertically);
            }
        }

        public bool WrapHorizontally { get; set; } = false;

        public bool WrapVertically { get; set; } = false;

        public BackgroundImage(Texture texture, bool isEnabled)
            : base(isEnabled, true)
        {
            Texture = texture;
        }

        protected override void Draw(RendererBatch batch)
        {
            for (int i = 0; i < DrawableRegion.Width * DrawableRegion.Height; i++)
            {
                var x = (!WrapVertically ? DrawableRegion.X : 0f) + (i % DrawableRegion.Width);
                var y = (!WrapHorizontally ? DrawableRegion.Y : 0f) + (i / DrawableRegion.Width);
                var position = DrawPosition + new Vector2(x * Bounds.Width, y * Bounds.Height);

                Texture.Draw(batch, position, Vector2.Zero, Vector2.One, 0f, Color * MathHelper.Clamp(Opacity, 0f, 1f), SpriteEffects);
            }
        }

        protected override void OnEntityAdded()
        {
            ParentLayer.Camera.OnCameraUpdated += UpdateDrawableRegion;
            UpdateDrawableRegion(ParentLayer.Camera);
        }

        protected override void OnEntityRemoved()
        {
            ParentLayer.Camera.OnCameraUpdated -= UpdateDrawableRegion;
        }

        private void UpdateDrawableRegion(Camera camera)
        {
            var x1 = (int)Math.Floor((camera.Bounds.Left - DrawPosition.X) / Bounds.Width);
            var y1 = (int)Math.Floor((camera.Bounds.Top - DrawPosition.Y) / Bounds.Height);
            var x2 = (int)Math.Ceiling((camera.Bounds.Right + DrawPosition.X) / Bounds.Width);
            var y2 = (int)Math.Ceiling((camera.Bounds.Bottom + DrawPosition.Y) / Bounds.Height);

            var width = !WrapVertically ? x2 - x1 : 1;
            var height = !WrapHorizontally ? y2 - y1 : 1;

            DrawableRegion = new Rectangle(x1, y1, width, height);
        }
    }
}
