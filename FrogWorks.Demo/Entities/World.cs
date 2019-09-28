using Microsoft.Xna.Framework;

namespace FrogWorks.Demo.Entities
{
    public class World : Entity
    {
        protected BitFlagMapCollider Map => Collider.As<BitFlagMapCollider>();

        public Vector2 Absolute => Map.AbsolutePosition;

        public int CellWidth => Map.CellWidth;

        public int CellHeight => Map.CellHeight;

        public World(int columns, int rows, int tileWidth, int tileHeight)
            : base()
        {
            Collider = new BitFlagMapCollider(columns, rows, tileWidth, tileHeight);
            GenerateMap();
        }

        public World(TileMapContainer container)
            : this(container.Columns, container.Rows, container.TileWidth, container.TileHeight) { }

        public bool IsSolid(float x, float y, float width, float height)
            => Map.Collide(new RectangleF(x, y, width, height), BitFlag.FlagA);

        public bool IsJumpThru(float x, float y, float width, float height)
        {
            var hitbox = new RectangleF(x, y, width, height);

            if (Map.Collide(hitbox, BitFlag.FlagB))
            {
                var pA = Map.Place(hitbox).Max();
                var pB = Map.Place(new RectangleF(x, y, width, height - 1)).Max();
                if (pB.Y < pA.Y) return true;
            }

            return false;
        }

        public bool IsAboveJumpThru(float x, float y, float width, float height, float xVel, float yVel)
        {
            var hitbox = new RectangleF(x, y, width, height);

            if (Map.Collide(hitbox, BitFlag.FlagB))
            {
                var pA = Map.Place(hitbox).Max();
                var pB = Map.Place(new RectangleF(x - xVel, y - yVel, width, height)).Max();
                if (pB.Y < pA.Y) return true;
            }

            return false;
        }

        public void Configure(int x, int y, int tileWidth, int tileHeight, int index)
        {
            var flag = BitFlag.None;

            switch (index)
            {
                case 1: flag |= BitFlag.FlagB; break;
                case 15: flag |= BitFlag.FlagA; break;
                default: break;
            }

            Map.Fill(flag, x, y, 1, 1);
        }

        protected override void BeforeDraw(RendererBatch batch)
        {
            Map.Draw(batch, Color.DarkGreen, Color.LimeGreen);
        }

        void GenerateMap()
        {
            Map.Fill(BitFlag.FlagA, 0, 0, Map.Columns, 1);
            Map.Fill(BitFlag.FlagA, 0, 0, 1, Map.Rows);
            Map.Fill(BitFlag.FlagA, 0, Map.Rows - 1, Map.Columns, 1);
            Map.Fill(BitFlag.FlagA, Map.Columns - 1, 0, 1, Map.Rows);
        }
    }
}
