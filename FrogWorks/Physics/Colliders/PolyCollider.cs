using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public sealed class PolyCollider : ShapeCollider
    {
        Polygon _poly;
        Vector2[] _vertices;
        Vector2 _size;

        protected internal override Shape Shape => _poly;

        public Vector2 Scale
        {
            get { return _poly.Scale; }
            set
            {
                value = value.Abs();

                if (_poly.Scale != value)
                {
                    _poly.Scale = value;
                    OnTransformedInternally();
                }
            }
        }

        public float Angle
        {
            get { return _poly.Angle; }
            set
            {
                if (_poly.Angle != value)
                {
                    _poly.Angle = value;
                    OnTransformedInternally();
                }
            }
        }

        public float AngleInDegrees
        {
            get { return MathHelper.ToDegrees(Angle); }
            set { Angle = MathHelper.ToRadians(value); }
        }

        public override Vector2 Size
        {
            get
            {
                var transform = _poly.GetVertices();
                return transform.Max() - transform.Min();
            }
            set { Scale = value.Divide(_size); }
        }

        public override Vector2 Min
        {
            get { return _poly.GetVertices().Min(); }
            set
            {
                var offset = AbsolutePosition - _poly.GetVertices().Min();
                AbsolutePosition = value - offset;
            }
        }

        public override Vector2 Max
        {
            get { return _poly.GetVertices().Max(); }
            set
            {
                var offset = AbsolutePosition - _poly.GetVertices().Max();
                AbsolutePosition = value - offset;
            }
        }

        public PolyCollider(Vector2[] vertices)
            : this(Vector2.Zero, vertices)
        {
        }

        public PolyCollider(float x, float y, Vector2[] vertices)
            : this(new Vector2(x, y), vertices)
        {
        }

        public PolyCollider(float x, float y, float width, float height)
            : this(new Vector2(x, y), new Vector2(width, height))
        {
        }

        public PolyCollider(Vector2 position, Vector2[] vertices)
            : base(position)
        {
            _poly = new Polygon(AbsolutePosition, vertices);
            _vertices = _poly.GetVertices();
            _size = _vertices.Max() - _vertices.Min();
        }

        public PolyCollider(Vector2 position, Vector2 size)
            : this(
                position,
                new Vector2[]
                {
                    Vector2.Zero,
                    Vector2.UnitX * size,
                    Vector2.One * size,
                    Vector2.UnitY * size
                })
        {
        }

        public override Collider Clone()
        {
            return new PolyCollider(Position, _vertices)
            {
                Scale = Scale,
                Angle = Angle
            };
        }
    }
}
