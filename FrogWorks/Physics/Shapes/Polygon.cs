using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public sealed class Polygon : Shape
    {
        Vector2[] _vertices, _transformed;
        Vector2 _origin, _scale;
        float _angle;

        public Vector2 this[int index] => _transformed[index];

        public int Count => _vertices.Length;

        public override Vector2 Center
        {
            get { return Position + _origin; }
            set { Position = value - _origin; }
        }

        public Vector2 Scale
        {
            get { return _scale; }
            set
            {
                if (_scale != value)
                {
                    _scale = value;
                    MarkAsDirty();
                    RecalculateVertices();
                }
            }
        }

        public float Angle
        {
            get { return _angle; }
            set
            {
                if (_angle != value)
                {
                    _angle = value;
                    MarkAsDirty();
                    RecalculateVertices();
                }
            }
        }

        public float AngleInDegrees
        {
            get { return MathHelper.ToDegrees(Angle); }
            set { Angle = MathHelper.ToRadians(value); }
        }

        public Polygon(Vector2[] vertices)
            : this(vertices.Min(), vertices, Vector2.One, 0f)
        {
        }

        public Polygon(float x, float y, Vector2[] vertices)
            : this(new Vector2(x, y), vertices, Vector2.One, 0f)
        {
        }

        public Polygon(Vector2 position, Vector2[] vertices)
            : this(position, vertices, Vector2.One, 0f)
        {
        }

        public Polygon(Vector2 position, Vector2[] vertices, Vector2 scale, float angle)
            : base(position)
        {
            _vertices = vertices.ToOrigin().ToConvexHull();
            _origin = (_vertices.Max() - _vertices.Min()) * .5f;
            _scale = scale;
            _angle = angle;
        }

        void RecalculateVertices()
        {
            _transformed = _vertices.Transform(Position, _origin, _scale, _angle, false);
        }

        protected override Rectangle RecalculateBounds()
        {
            var min = _transformed.Min();
            var max = _transformed.Max();

            return new Rectangle(min.ToPoint(), (max - min).ToPoint());
        }

        public override void Draw(
            RendererBatch batch,
            Color strokeColor,
            Color? fillColor = null)
        {
            batch.DrawPrimitives(p =>
            {
                if (fillColor.HasValue)
                    p.FillPolygon(_transformed, fillColor.Value);

                p.DrawPolygon(_transformed, strokeColor);
            });
        }

        public override Shape Clone()
        {
            return new Polygon(Position, _vertices, _scale, _angle);
        }

        protected override void OnTranslated()
        {
            RecalculateVertices();
        }
    }
}
