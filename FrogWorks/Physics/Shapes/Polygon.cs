using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public sealed class Polygon : Shape
    {
        Vector2[] _vertices, _transform;
        Vector2 _origin, _scale;
        float _angle;
        bool _isDirty = true;

        Vector2[] Vertices
        {
            get
            {
                RecalculateVertices();
                return _transform;
            }
        }

        public Vector2 this[int index] => Vertices[index];

        public int Count => Vertices.Length;

        public Vector2 Scale
        {
            get { return _scale; }
            set
            {
                if (_scale != value)
                {
                    _scale = value;
                    _isDirty = true;
                    MarkAsDirty();
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
                    _isDirty = true;
                    MarkAsDirty();
                }
            }
        }

        public float AngleInDegrees
        {
            get { return Angle.ToDeg(); }
            set { Angle = value.ToRad(); }
        }

        public Vector2 Min => Vertices.Min();

        public Vector2 Max => Vertices.Max();

        public override Vector2 Center
        {
            get { return Position + _origin; }
            set { Position = value - _origin; }
        }

        public Polygon(Vector2 position, Vector2[] vertices)
            : this(position, Vector2.One, 0f, vertices)
        {
        }

        public Polygon(float x, float y, Vector2[] vertices)
            : this(new Vector2(x, y), Vector2.One, 0f, vertices)
        {
        }

        public Polygon(Vector2 position, Vector2 scale, float angle, Vector2[] vertices)
            : base(position)
        {
            _scale = scale;
            _angle = angle;
            _vertices = vertices.ToOrigin().ToConvexHull();
            _origin = vertices.Center();
        }

        public Polygon(float x, float y, Vector2 scale, float angle, Vector2[] vertices)
            : this(new Vector2(x, y), scale, angle, vertices)
        {
        }

        public override bool Contains(Vector2 point)
        {
            var inside = false;

            for (int i = 0; i < Count; i++)
            {
                var start = this[i];
                var end = this[(i + 1).Mod(Count)];
                var edge = end - start;

                if ((start.Y > point.Y) != (end.Y > point.Y))
                {
                    var area = edge.X * (point.Y - start.Y) / edge.Y + start.X;
                    if (point.X < area) inside = !inside;
                }
            }

            return inside;
        }

        public override Vector2[] GetVertices()
        {
            return Vertices;
        }

        public override void Draw(RendererBatch batch, Color color, Color fill)
        {
            batch.DrawPrimitives(p =>
            {
                if (fill != default)
                    p.FillPolygon(Vertices, fill);

                p.DrawPolygon(Vertices, color);
            });
        }

        public override Shape Clone()
        {
            return new Polygon(Position, Scale, Angle, _vertices);
        }

        protected override Rectangle RecalculateBounds()
        {
            return new Rectangle(Min.ToPoint(), (Max - Min).ToPoint());
        }

        protected override void OnTranslate()
        {
            _isDirty = true;
        }

        void RecalculateVertices()
        {
            if (_isDirty)
            {
                _transform = _vertices.Transform(Position, _origin, _scale, _angle, false);
                _isDirty = false;
            }
        }
    }
}
