using Microsoft.Xna.Framework;

namespace FrogWorks.Demo
{
    public class Playfield : Entity
    {
        SimpleMapCollider Map => Collider.As<SimpleMapCollider>();

        public Playfield(int width, int height, int tileWidth, int tileHeight)
        {
            Collider = new SimpleMapCollider(0, 0, width, height, tileWidth, tileHeight);
            CreateBorders();
        }

        protected override void AfterDraw(RendererBatch batch)
        {
            Collider.Draw(batch, Color.Blue);
        }

        void CreateBorders()
        {
            var width = Map.Columns;
            var height = Map.Rows;

            Map.Fill(true, 0, 0, width, 1);
            Map.Fill(true, 0, 0, 1, height);
            Map.Fill(true, 0, height - 1, width, 1);
            Map.Fill(true, width - 1, 0, 1, height);

            Map.Fill(true, width / 2, height / 2, 2, 1);
        }

        public bool Check(Collider col, float off, bool vertical, out Vector2 amt)
        {
            amt = default;

            if (off == 0) return false;

            var unit = vertical ? Vector2.UnitY : Vector2.UnitX;
            var len = (vertical ? col.Height : col.Width) / 2f;
            var norm = unit * off.Sign();
            var dist = len + off.Abs();

            if (Map.CastRay(col.Center, norm, dist, out var hit))
            {
                var dpt = hit.Depth;
                amt = dpt * norm;
                return true;
            }

            return false;
        }
    }
}
