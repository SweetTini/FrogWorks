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
            var collider = new BitFlagMapCollider(columns, rows, tileWidth, tileHeight);

            collider.Fill(BitFlag.FlagA, 0, 0, columns, 1);
            collider.Fill(BitFlag.FlagA, 0, 0, 1, rows);
            collider.Fill(BitFlag.FlagA, 0, rows - 1, columns, 1);
            collider.Fill(BitFlag.FlagA, columns - 1, 0, 1, rows);

            Collider = collider;
        }

        protected override void BeforeDraw(RendererBatch batch)
        {
            Collider.Draw(batch, Color.DarkGreen, Color.LimeGreen);
        }

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
    }
}
