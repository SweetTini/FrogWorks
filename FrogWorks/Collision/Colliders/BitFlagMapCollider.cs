using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace FrogWorks
{
    public class BitFlagMapCollider : MapCollider<BitFlag>
    {
        private Dictionary<BitFlag, MapColorDefinition> _colors;

        public BitFlagMapCollider(Point size, Point cellSize)
            : base(size, cellSize, Vector2.Zero)
        {
            _colors = new Dictionary<BitFlag, MapColorDefinition>();
        }

        public BitFlagMapCollider(Point size, Point cellSize, Vector2 offset)
            : base(size, cellSize, offset)
        {
            _colors = new Dictionary<BitFlag, MapColorDefinition>();
        }

        public BitFlagMapCollider(int columns, int rows, int cellWidth, int cellHeight)
            : base(columns, rows, cellWidth, cellHeight, 0f, 0f)
        {
            _colors = new Dictionary<BitFlag, MapColorDefinition>();
        }

        public BitFlagMapCollider(int columns, int rows, int cellWidth, int cellHeight, float offsetX, float offsetY)
            : base(columns, rows, cellWidth, cellHeight, offsetX, offsetY)
        {
            _colors = new Dictionary<BitFlag, MapColorDefinition>();
        }

        public bool Collide(Vector2 point, BitFlag flags)
            => Validate(Place(point), flags, (p, e) => Validate(p, s => s.Contains(point)));

        public bool Collide(float x, float y, BitFlag flags) => Collide(new Vector2(x, y), flags);

        public bool Collide(Ray2D ray, BitFlag flags)
            => Validate(Place(ray), flags, (p, e) => Validate(p, s => ray.Cast(s)));

        public bool Collide(Shape shape, BitFlag flags)
            => Validate(Place(shape), flags, (p, e) => Validate(p, s => shape.Collide(s)));

        public bool Collide(Collider collider, BitFlag flags)
        {
            var isValid = collider != null
                && !Equals(collider)
                && collider.IsCollidable
                && collider is ShapeCollider;

            return isValid && Validate(Place(collider), flags, 
                (p, e) => Validate(p, s => (collider as ShapeCollider).Collide(s)));
        }

        public bool Collide(Entity entity, BitFlag flags) => Collide(entity?.Collider, flags);

        public override Collider Clone()
        {
            var collider = new BitFlagMapCollider(MapSize, CellSize, Position);
            collider.Populate(Map.ToArray());
            return collider;
        }

        public bool Validate(Point point, BitFlag flags, Func<Point, BitFlag, bool> predicate)
            => Validate(Extensions.Enumerate(point), flags, predicate);

        public bool Validate(IEnumerable<Point> points, BitFlag flags, Func<Point, BitFlag, bool> predicate)
        {
            if (IsCollidable)
            {
                foreach (var point in points)
                {
                    var flag = Map[point.X, point.Y];
                    if (flag.Equals(Map.Empty)) continue;
                    if ((flag & flags) != 0 && predicate(point, flag)) return true;
                }
            }

            return false;
        }

        public override Shape ShapeAt(Point point)
        {
            return !Map.IsEmpty(point)
                ? new RectangleF(
                    AbsolutePosition + (point * CellSize).ToVector2(),
                    CellSize.ToVector2())
                : null;
        }

        public void DefineColors(BitFlag flags, Color stroke, Color? fill = null)
            => _colors.Add(flags, new MapColorDefinition(stroke, fill));

        public void ClearColors() => _colors.Clear();

        protected override void DrawShapeAt(Point point, RendererBatch batch, Color stroke, Color? fill = null)
        {
            MapColorDefinition color;
            if (!_colors.TryGetValue(ElementAt(point), out color))
                color = new MapColorDefinition(stroke, fill);
            base.DrawShapeAt(point, batch, color.Stroke, color.Fill);
        }
    }

    internal struct MapColorDefinition
    {
        public Color Stroke { get; set; }

        public Color? Fill { get; set; }

        public MapColorDefinition(Color stroke, Color? fill = null)
        {
            Stroke = stroke;
            Fill = fill;
        }
    }

    [Flags]
    public enum BitFlag
    {
        None = 0,
        FlagA = 1,
        FlagB = 2,
        FlagC = 4,
        FlagD = 8,
        FlagE = 16,
        FlagF = 32,
        FlagG = 64,
        FlagH = 128,
        FlagI = 256,
        FlagJ = 512,
        FlagK = 1024,
        FlagL = 2048,
        FlagM = 4096,
        FlagN = 8192,
        FlagO = 16384,
        FlagP = 32768,
        FlagQ = 65536,
        FlagR = 131072,
        FlagS = 262144,
        FlagT = 524288,
        FlagU = 1048576,
        FlagV = 2097152,
        FlagW = 4194304,
        FlagX = 8388608,
        FlagY = 16777216,
        FlagZ = 33554432,
        All = 67108863
    }
}
