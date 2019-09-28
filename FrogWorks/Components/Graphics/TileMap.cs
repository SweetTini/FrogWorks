using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FrogWorks
{
    public class TileMap : Component
    {
        private Vector2 _position;
        private Rectangle _drawableRegion;
        private bool _isRegistered;

        protected Map<Texture> TextureMap { get; private set; }

        public int Columns => TextureMap.Columns;

        public int Rows => TextureMap.Rows;

        public int TileWidth { get; private set; }

        public int TileHeight { get; private set; }

        public Rectangle Bounds => new Rectangle(0, 0, Columns * TileWidth, Rows * TileHeight);

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

        public TileMap(int columns, int rows, int tileWidth, int tileHeight)
            : base(false, true)
        {
            TextureMap = new Map<Texture>(columns, rows);
            TileWidth = tileWidth;
            TileHeight = tileHeight;
        }

        protected override void Draw(RendererBatch batch)
        {
            for (int i = 0; i < _drawableRegion.Width * _drawableRegion.Height; i++)
            {
                var x = _drawableRegion.Left + (i % _drawableRegion.Width);
                var y = _drawableRegion.Top + (i / _drawableRegion.Width);
                var position = DrawPosition + new Vector2(x * TileWidth, y * TileHeight);

                TextureMap[x, y]?.Draw(batch, position, Vector2.Zero, Vector2.One, 0f, Color * Opacity.Clamp(0f, 1f), SpriteEffects);
            }
        }

        protected override void OnAdded() => AddCameraUpdateEvent();

        protected override void OnRemoved() => RemoveCameraUpdateEvent();

        protected override void OnEntityAdded() => AddCameraUpdateEvent();

        protected override void OnEntityRemoved() => RemoveCameraUpdateEvent();

        protected override void OnLayerAdded() => AddCameraUpdateEvent();

        protected override void OnLayerRemoved() => RemoveCameraUpdateEvent();

        protected override void OnTransformed() => UpdateDrawableRegion(Layer.Camera);

        public void Populate(TileSet tileSet, int[,] tiles, int offsetX = 0, int offsetY = 0)
        {
            var tileColumns = tiles.GetLength(0);
            var tileRows = tiles.GetLength(1);

            for (int i = 0; i < tileColumns * tileRows; i++)
            {
                var x = i % tileColumns;
                var y = i / tileColumns;
                var index = tiles[x, y];
                TextureMap[x + offsetX, y + offsetY] = tileSet[index];
            }
        }

        public void Overlay(TileSet tileSet, int[,] tiles, int offsetX = 0, int offsetY = 0)
        {
            var tileColumns = tiles.GetLength(0);
            var tileRows = tiles.GetLength(1);

            for (int i = 0; i < tileColumns * tileRows; i++)
            {
                var x = i % tileColumns;
                var y = i / tileColumns;
                var index = tiles[x, y];

                if (index >= 0)
                    TextureMap[x + offsetX, y + offsetY] = tileSet[index];
            }
        }

        public void Fill(Texture tile, int x, int y, int columns, int rows)
        {
            var x1 = Math.Max(x, 0);
            var y1 = Math.Max(y, 0);
            var x2 = Math.Min(x + columns, Columns);
            var y2 = Math.Min(y + rows, Rows);

            var tileColumns = x2 - x1;
            var tileRows = y2 - y1;

            for (int i = 0; i < tileColumns * tileRows; i++)
            {
                var tx = x1 + (i % tileColumns);
                var ty = y1 + (i / tileColumns);

                TextureMap[tx, ty] = tile;
            }
        }

        public void Clear()
        {
            TextureMap.Clear();
        }

        public void Resize(int columns, int rows)
        {
            TextureMap.Resize(columns, rows);
        }

        public void Resize(int x1, int y1, int x2, int y2)
        {
            TextureMap.Resize(x1, y1, x2, y2);
        }

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

            var x1 = (int)Math.Max(Math.Floor((camera.View.Left - DrawPosition.X) / TileWidth), 0);
            var y1 = (int)Math.Max(Math.Floor((camera.View.Top - DrawPosition.Y) / TileHeight), 0);
            var x2 = (int)Math.Min(Math.Ceiling((camera.View.Right + DrawPosition.X) / TileWidth), Columns);
            var y2 = (int)Math.Min(Math.Ceiling((camera.View.Bottom + DrawPosition.Y) / TileHeight), Rows);

            _drawableRegion = new Rectangle(x1, y1, x2 - x1, y2 - y1);
        }
    }
}
