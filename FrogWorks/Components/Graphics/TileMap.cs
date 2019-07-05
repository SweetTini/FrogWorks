using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace FrogWorks
{
    public class TileMap : Component
    {
        protected Map<Texture> TextureMap { get; private set; }

        public int Columns => TextureMap.Columns;

        public int Rows => TextureMap.Rows;

        public int TileWidth { get; private set; }

        public int TileHeight { get; private set; }

        public Rectangle Bounds => new Rectangle(0, 0, Columns * TileWidth, Rows * TileHeight);

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
            TileHeight = TileHeight;
        }

        public override void Draw(RendererBatch batch)
        {
            for (int i = 0; i < DrawableRegion.Width * DrawableRegion.Height; i++)
            {
                var x = DrawableRegion.Left + (i % DrawableRegion.Width);
                var y = DrawableRegion.Top + (i / DrawableRegion.Width);
                var position = DrawPosition + new Vector2(x * TileWidth, y * TileHeight);

                TextureMap[x, y]?.Draw(batch, position, Vector2.Zero, Vector2.One, 0f, Color, SpriteEffects);
            }
        }

        public override void OnEntityAdded(Entity entity)
        {
            if (entity.Equals(Entity) && Entity.Layer != null)
            {
                var camera = Entity.Layer.Camera;
                UpdateDrawableRegion(camera);
                camera.OnCameraUpdated += UpdateDrawableRegion;
            }
        }

        public override void OnEntityRemoved(Entity entity)
        {
            if (entity.Equals(Entity) && Entity.Layer != null)
                Entity.Layer.Camera.OnCameraUpdated -= UpdateDrawableRegion;
        }

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
                var tx = i % tileColumns;
                var ty = i / tileColumns;
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

        private void UpdateDrawableRegion(Camera camera)
        {
            var x1 = (int)Math.Max(Math.Floor((camera.Bounds.Left - DrawPosition.X) / Bounds.Width), 0);
            var y1 = (int)Math.Max(Math.Floor((camera.Bounds.Top - DrawPosition.Y) / Bounds.Height), 0);
            var x2 = (int)Math.Min(Math.Ceiling((camera.Bounds.Right + DrawPosition.X) / Bounds.Width), Columns);
            var y2 = (int)Math.Min(Math.Ceiling((camera.Bounds.Bottom + DrawPosition.Y) / Bounds.Height), Rows);

            DrawableRegion = new Rectangle(x1, y1, x2 - x1, y2 - y1);
        }
    }
}
