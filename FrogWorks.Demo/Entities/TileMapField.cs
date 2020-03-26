using Microsoft.Xna.Framework;
using System.Linq;

namespace FrogWorks.Demo
{
    public class TileMapField : Entity
    {
        SimpleMapCollider Map { get; set; }

        public TileMapField(
            int mapWidth, int mapHeight, 
            int tileWidth, int tileHeight)
            : base()
        {
            Map = new SimpleMapCollider(
                mapWidth, mapHeight, 
                tileWidth, tileHeight);

            Collider = Map;
            InitializeMap();
        }

        void InitializeMap()
        {
            Map.Fill(true, 0, 0, 1, Map.Rows);
            Map.Fill(true, 0, 0, Map.Rows, 1);
            Map.Fill(true, Map.Columns - 1, 0, 1, Map.Rows);
            Map.Fill(true, 0, Map.Rows - 1, Map.Columns, 1);
            Map.Fill(true, Map.Columns / 2 - 1, Map.Rows / 2, 2, 1);
        }

        protected override void BeforeDraw(RendererBatch batch)
        {
            Collider.Draw(batch, ColorEX.FromRGB(0, 0, 128));
        }

        public bool Overlaps(Entity entity, Vector2 velocity, Vector2 axis, out Vector2 depth)
        {
            depth = Vector2.Zero;

            if (Map.Overlaps(entity, out var result))
            {
                var hits = result.Hits
                    .Where(x => x.Normal.Abs() == axis)
                    .Select(x => x.Translation)
                    .ToList();

                var vector = axis == Vector2.UnitX
                    ? velocity.X
                    : velocity.Y;

                if (hits.Any())
                {
                    depth = vector < 0
                    ? hits.Max()
                    : hits.Min();

                    return true;
                }
            }

            return false;
        }
    }
}
