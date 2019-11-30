using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FrogWorks
{
    public abstract class TiledGraphicsComponent : Component
    {
        private Vector2 _position;
        private Rectangle _drawRegion;
        private bool _linked;

        public Texture Texture { get; protected set; }

        public Point TileSize { get; protected set; }

        public int TileWidth => TileSize.X;

        public int TileHeight => TileSize.Y;

        public Vector2 Position
        {
            get { return _position; }
            set
            {
                if (_position == value) return;
                _position = value;
                UpdateDrawRegion(Layer?.Camera);
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

        public bool WrapHorizontally { get; set; }

        public bool WrapVertically { get; set; }

        protected TiledGraphicsComponent(bool isEnabled)
            : base(isEnabled, true)
        {
        }

        protected sealed override void Draw(RendererBatch batch)
        {
            for (int i = 0; i < _drawRegion.Width * _drawRegion.Height; i++)
            {
                var x = _drawRegion.Left + (i % _drawRegion.Width);
                var y = _drawRegion.Top + (i / _drawRegion.Width);
                var position = DrawPosition + new Vector2(x * TileWidth, y * TileHeight);

                GetTile(x, y)?.Draw(batch, position, Vector2.Zero, Vector2.One, 0f,
                                    Color * Opacity.Clamp(0f, 1f), SpriteEffects);
            }
        }

        protected abstract Texture GetTile(int x, int y);

        protected override void OnAdded() => LinkCamera();

        protected override void OnRemoved() => UnlinkCamera();

        protected override void OnEntityAdded() => LinkCamera();

        protected override void OnEntityRemoved() => UnlinkCamera();

        protected override void OnLayerAdded() => LinkCamera();

        protected override void OnLayerRemoved() => UnlinkCamera();

        protected override void OnTransformed() => UpdateDrawRegion(Layer.Camera);

        private void UpdateDrawRegion(Camera camera)
        {
            if (camera == null) return;

            var x1 = (int)((camera.View.Left - DrawPosition.X.Abs()) / TileWidth).Floor();
            var y1 = (int)((camera.View.Top - DrawPosition.Y.Abs()) / TileHeight).Floor();
            var x2 = (int)((camera.View.Right + DrawPosition.X.Abs()) / TileWidth).Ceiling();
            var y2 = (int)((camera.View.Bottom + DrawPosition.Y.Abs()) / TileHeight).Ceiling();

            _drawRegion = new Rectangle(x1, y1, x2 - x1, y2 - y1);
        }

        private void LinkCamera()
        {
            if (!_linked && Layer != null)
            {
                Layer.Camera.OnChanged += UpdateDrawRegion;
                UpdateDrawRegion(Layer.Camera);
                _linked = true;
            }
        }

        private void UnlinkCamera()
        {
            if (_linked && Layer != null)
            {
                Layer.Camera.OnChanged -= UpdateDrawRegion;
                _linked = false;
            }
        }
    }
}
