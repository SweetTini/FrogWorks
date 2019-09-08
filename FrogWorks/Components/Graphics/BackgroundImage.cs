using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FrogWorks
{
    public class BackgroundImage : Component
    {
        private Vector2 _position;
        private Rectangle _drawableRegion;
        private bool _isRegistered;

        public Texture Texture { get; protected set; }

        public Rectangle Bounds => new Rectangle(0, 0, Texture.Width, Texture.Height);

        public Vector2 Position
        {
            get { return _position; }
            set
            {
                if (value == _position) return;
                _position = value;
                UpdateDrawableRegion(Layer?.Camera);
            }
        }

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
            for (int i = 0; i < _drawableRegion.Width * _drawableRegion.Height; i++)
            {
                var x = (!WrapVertically ? _drawableRegion.X : 0f) + (i % _drawableRegion.Width);
                var y = (!WrapHorizontally ? _drawableRegion.Y : 0f) + (i / _drawableRegion.Width);
                var position = DrawPosition + new Vector2(x * Bounds.Width, y * Bounds.Height);

                Texture.Draw(batch, position, Vector2.Zero, Vector2.One, 0f, Color * Opacity.Clamp(0f, 1f), SpriteEffects);
            }
        }

        protected override void OnAdded() => AddCameraUpdateEvent();

        protected override void OnRemoved() => RemoveCameraUpdateEvent();

        protected override void OnEntityAdded() => AddCameraUpdateEvent();

        protected override void OnEntityRemoved() => RemoveCameraUpdateEvent();

        protected override void OnLayerAdded() => AddCameraUpdateEvent();

        protected override void OnLayerRemoved() => RemoveCameraUpdateEvent();

        private void AddCameraUpdateEvent()
        {
            if (!_isRegistered && Layer != null)
            {
                Layer.Camera.OnCameraUpdated += UpdateDrawableRegion;
                UpdateDrawableRegion(Layer.Camera);
                _isRegistered = true;
            }
        }

        private void RemoveCameraUpdateEvent()
        {
            if (_isRegistered && Layer != null)
            {
                Layer.Camera.OnCameraUpdated -= UpdateDrawableRegion;
                _isRegistered = false;
            }
        }

        private void UpdateDrawableRegion(Camera camera)
        {
            if (camera == null) return;

            var x1 = (int)Math.Floor((camera.View.Left - DrawPosition.X) / Bounds.Width);
            var y1 = (int)Math.Floor((camera.View.Top - DrawPosition.Y) / Bounds.Height);
            var x2 = (int)Math.Ceiling((camera.View.Right + DrawPosition.X) / Bounds.Width);
            var y2 = (int)Math.Ceiling((camera.View.Bottom + DrawPosition.Y) / Bounds.Height);

            var width = !WrapVertically ? x2 - x1 : 1;
            var height = !WrapHorizontally ? y2 - y1 : 1;

            _drawableRegion = new Rectangle(x1, y1, width, height);
        }
    }
}
