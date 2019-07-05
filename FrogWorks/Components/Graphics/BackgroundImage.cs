using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FrogWorks
{
    public class BackgroundImage : Component
    {
        public Texture Texture { get; protected set; }

        public Rectangle Bounds => new Rectangle(0, 0, Texture.Width, Texture.Height);

        public Rectangle DrawableRegion { get; private set; }

        public Vector2 Position { get; set; }

        public float X
        {
            get { return Position.X; }
            set { Position = new Vector2(value, Position.Y); }
        }

        public float Y
        {
            get { return Position.Y; }
            set { Position = new Vector2(Position.X, value); }
        }

        public Vector2 DrawPosition
        {
            get { return Position + (Entity?.Position ?? Vector2.Zero); }
            set { Position = value - (Entity?.Position ?? Vector2.Zero); }
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

        public override void Draw(RendererBatch batch)
        {
            for (int i = 0; i < DrawableRegion.Width * DrawableRegion.Height; i++)
            {
                var x = (!WrapVertically ? DrawableRegion.X : 0f) + (i % DrawableRegion.Width);
                var y = (!WrapHorizontally ? DrawableRegion.Y : 0f) + (i / DrawableRegion.Width);
                var position = DrawPosition + new Vector2(x * Bounds.Width, y * Bounds.Height);

                Texture.Draw(batch, position, Vector2.Zero, Vector2.One, 0f, Color * MathHelper.Clamp(Opacity, 0f, 1f), SpriteEffects);
            }
        }

        public override void OnEntityAdded(Entity entity)
        {
            var camera = entity.Layer.Camera;
            camera.OnCameraUpdated += UpdateDrawableRegion;
            UpdateDrawableRegion(camera);
        }

        public override void OnEntityRemoved(Entity entity)
        {
            entity.Layer.Camera.OnCameraUpdated -= UpdateDrawableRegion;
        }

        public override void OnLayerChanged(Layer layer, Layer lastLayer)
        {
            lastLayer.Camera.OnCameraUpdated -= UpdateDrawableRegion;
            layer.Camera.OnCameraUpdated += UpdateDrawableRegion;
            UpdateDrawableRegion(layer.Camera);
        }

        private void UpdateDrawableRegion(Camera camera)
        {
            var x1 = (int)Math.Floor((camera.Bounds.Left - DrawPosition.X) / Bounds.Width);
            var y1 = (int)Math.Floor((camera.Bounds.Top - DrawPosition.Y) / Bounds.Height);
            var x2 = (int)Math.Ceiling((camera.Bounds.Right + DrawPosition.X) / Bounds.Width);
            var y2 = (int)Math.Ceiling((camera.Bounds.Bottom + DrawPosition.Y) / Bounds.Height);

            if (WrapHorizontally) y2 = y1 + 1;
            if (WrapVertically) x2 = x1 + 1;

            DrawableRegion = new Rectangle(x1, y1, x2 - x1, y2 - y1);
        }
    }
}
