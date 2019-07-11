using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public class Polygon : Shape
    {
        private Vector2[] _vertices, _transform, _normals;
        private Vector2 _position, _origin, _scale;
        private float _angle;
        private bool _isDirty;

        public Vector2[] Vertices
        {
            get
            {
                UpdateVertices();
                return _transform;
            }
        }

        public Vector2[] Normal
        {
            get
            {
                UpdateVertices();
                return _normals;
            }
        }

        public int Count => _vertices.Length;

        public override Vector2 Position
        {
            get { return _position; }
            set
            {
                if (value == _position) return;
                _position = value;
                _isDirty = true;
            }
        }

        public Vector2 Origin
        {
            get { return _origin; }
            set
            {
                if (value == _origin) return;
                _origin = value;
                _isDirty = true;
            }
        }

        public Vector2 Scale
        {
            get { return _scale; }
            set
            {
                if (value == _scale) return;
                _scale = value;
                _isDirty = true;
            }
        }

        public float Angle
        {
            get { return _angle; }
            set
            {
                if (value == _angle) return;
                _angle = value;
                _isDirty = true;
            }
        }

        public float AngleInDegrees
        {
            get { return MathHelper.ToDegrees(Angle); }
            set { Angle = MathHelper.ToRadians(value); }
        }

        public override Rectangle Bounds => Vertices.ToRectangle();

        public Polygon(Vector2[] vertices)
        {
            _vertices = vertices.ToConvexHull();
            _origin = (vertices.Max() - vertices.Min()) / 2f;
            _scale = Vector2.One;

            UpdateVertices(true);
        }

        public override void Draw(RendererBatch batch, Color color, bool fill = false)
        {
            var shape = this;

            batch.DrawPrimitives((primitive) =>
            {
                if (fill) primitive.FillPolygon(shape.Vertices, color);
                else primitive.DrawPolygon(shape.Vertices, color);
            });
        }

        public override bool Contains(Vector2 point)
        {
            var inside = false;

            for (int i = 0; i < Count; i++)
            {
                var pointA = Vertices[i];
                var pointB = Vertices[(i + 1) % _vertices.Length];

                if ((pointA.Y > point.Y) != (pointB.Y > point.Y))
                {
                    var area = (pointB.X - pointA.X) * (point.Y - pointA.Y) / (pointB.Y - pointA.Y) + pointA.X;
                    if (point.X < area) inside = !inside;
                }
            }

            return inside;
        }

        private void UpdateVertices(bool forceUpdate = false)
        {
            if (_isDirty || forceUpdate)
            {
                _transform = _vertices.Transform(_position, _origin, _scale, _angle);
                _normals = _transform.Normalize();
                _isDirty = false;
            }
        }
    }
}
