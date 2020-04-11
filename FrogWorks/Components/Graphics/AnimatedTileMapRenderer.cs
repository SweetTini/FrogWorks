using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public class AnimatedTileMapRenderer : TileMapRenderer
    {
        float _timer;

        public AnimatedTileMapRenderer(Point size, Point tileSize)
            : base(size, tileSize, true)
        {
        }

        public AnimatedTileMapRenderer(
            int columns,
            int rows,
            int tileWidth,
            int tileHeight)
            : base(
                  new Point(columns, rows),
                  new Point(tileWidth, tileHeight),
                  true)
        {
        }

        protected override void Update(float deltaTime)
        {
            _timer += deltaTime;
        }

        protected override Texture GetTile(int x, int y)
        {
            if (WrapHorizontally) x = x.Mod(Columns);
            if (WrapVertically) y = y.Mod(Rows);

            var tile = Map[x, y];
            tile?.OffsetByTimer(_timer);
            return tile?.Texture;
        }

        public void Fill(
            TileSet tileSet,
            Animation animation,
            Point location,
            Point size)
        {
            var from = location.Max(Point.Zero);
            var to = (location + size).Min(Size);
            var area = to - from;

            animation.Loop = true;
            animation.MaxLoops = 0;

            for (int i = 0; i < area.X * area.Y; i++)
            {
                var x = location.X + (i % area.X);
                var y = location.Y + (i / area.X);

                Map[x, y] = new Tile(tileSet, animation);
            }
        }

        public void Fill(
            TileSet tileSet,
            Animation animation,
            int x,
            int y,
            int columns,
            int rows)
        {
            Fill(
                tileSet,
                animation,
                new Point(x, y),
                new Point(columns, rows));
        }

        public void Fill(
            TileSet tileSet,
            int[] frames,
            float frameStep,
            AnimationPlayMode playMode,
            Point location,
            Point size)
        {
            Fill(tileSet,
                new Animation(frames, frameStep, playMode),
                location,
                size);
        }

        public void Fill(
            TileSet tileSet,
            int[] frames,
            float frameStep,
            AnimationPlayMode playMode,
            int x,
            int y,
            int columns,
            int rows)
        {
            Fill(
                tileSet,
                new Animation(frames, frameStep, playMode),
                new Point(x, y),
                new Point(columns, rows));
        }

        public void Reset()
        {
            _timer = 0f;
        }
    }
}
