using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public class Polygon : Shape
    {
        private Vector2[] _vertices, _transform, _normals;
        private Vector2 _position, _size, _origin, _scale;
        private float _angle;
        private bool _isDirty;

        public Vector2[] Vertices => _vertices;

        public Vector2[] Transform
        {
            get
            {
                UpdateVertices();
                return _transform;
            }
        }

        public Vector2[] Normals
        {
            get
            {
                UpdateVertices();
                return _normals;
            }
        }

        public int Count { get; private set; }

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

        public Vector2 Size
        {
            get { return (_size * Scale.Abs()).Round(); }
            set { Scale = _size.Divide(value.Abs()); }
        }

        public float Width
        {
            get { return Size.X; }
            set { Size = new Vector2(value, Size.Y); }
        }

        public float Height
        {
            get { return Size.Y; }
            set { Size = new Vector2(Size.X, value); }
        }

        public override Rectangle Bounds => Transform.ToRectangle();

        public Polygon(Vector2[] vertices)
            : this(vertices.Min(), vertices)
        {
        }

        public Polygon(Vector2 position, Vector2[] vertices)
        {
            _vertices = vertices.ToConvexHull();
            _position = position;
            _size = vertices.Max() - vertices.Min();
            _origin = _size / 2f;
            _scale = Vector2.One;
            Count = _vertices.Length;

            UpdateVertices(true);
        }

        public override void Draw(RendererBatch batch, Color color, bool fill = false)
        {
            batch.DrawPrimitives((primitive) =>
            {
                if (fill) primitive.FillPolygon(Transform, color);
                else primitive.DrawPolygon(Transform, color);
            });
        }

        public override bool Contains(Vector2 point)
        {
            var inside = false;

            for (int i = 0; i < Count; i++)
            {
                var line = GetLine(i);

                if ((line.Startpoint.Y > point.Y) != (line.Endpoint.Y > point.Y))
                {
                    var area = line.Edge.X * (point.Y - line.Startpoint.Y) / line.Edge.Y + line.Startpoint.X;
                    if (point.X < area) inside = !inside;
                }
            }

            return inside;
        }

        public override Shape Clone()
        {
            return new Polygon(Position, _vertices)
            {
                Origin = Origin,
                Scale = Scale,
                Angle = Angle
            };
        }

        public override Proxy ToProxy()
        {
            return new Proxy(Transform);
        }

        internal Line GetLine(int index)
        {
            return new Line(_transform[index], _transform[(index + 1) % Count]);
        }

        internal Line GetClosestLine(Polygon other, int otherIndex)
        {
            var normal = other.Normals[otherIndex];
            var dotProd = Vector2.Dot(normal, _normals[0]);
            var index = 0;

            for (int i = 1; i < Count; i++)
            {
                var dp = Vector2.Dot(normal, _normals[i]);

                if (dp < dotProd)
                {
                    dotProd = dp;
                    index = i;
                }
            }

            return GetLine(index);
        }

        internal Plane GetPlane(int index)
        {
            return new Plane(_normals[index], Vector2.Dot(_normals[index], _transform[index]));
        }

        internal float GetMinIntersectionDepth(Polygon other, out int index)
        {
            index = -1;

            var minDepth = float.MinValue;

            for (int i = 0; i < Count; i++)
            {
                var plane = GetPlane(i);
                var supportIndex = GetSupport(-plane.Normal);
                var depth = plane.Distance(other.Transform[supportIndex]);

                if (depth > minDepth)
                {
                    minDepth = depth;
                    index = i;
                }
            }

            return minDepth;
        }

        private int GetSupport(Vector2 direction)
        {
            var index = 0;
            var dotProd = Vector2.Dot(_transform[0], direction);

            for (int i = 1; i < Count; i++)
            {
                var dp = Vector2.Dot(_transform[i], direction);

                if (dp > dotProd)
                {
                    dotProd = dp;
                    index = i;
                }
            }

            return index;
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

    internal struct Line
    {
        public Vector2 Startpoint { get; set; }

        public Vector2 Endpoint { get; set; }

        public Vector2 Edge => Endpoint - Startpoint;

        public Line(Vector2 start, Vector2 end)
        {
            Startpoint = start;
            Endpoint = end;
        }

        public Manifold KeepDeepestIntersection(Plane plane)
        {
            var hit = new Manifold();

            var distanceA = plane.Distance(Startpoint);
            var distanceB = plane.Distance(Endpoint);

            if (distanceA < 0f || distanceB < 0f)
            {
                hit.Depth = -(distanceA < distanceB ? distanceA : distanceB);
                hit.Normal = plane.Normal;
                hit.ContactPoint = distanceA < distanceB ? Startpoint : Endpoint;
            }

            return hit;
        }

        public bool TryClipping(Polygon polygon, int index, out Plane plane)
        {
            plane = new Plane();

            var polyLine = polygon.GetLine(index);
            var edgeNormal = Vector2.Normalize(polyLine.Edge);
            var leftPlane = new Plane(-edgeNormal, Vector2.Dot(-edgeNormal, polyLine.Startpoint));
            var rightPlane = new Plane(edgeNormal, Vector2.Dot(edgeNormal, polyLine.Endpoint));

            if (!Clip(leftPlane) || !Clip(rightPlane))
                return false;

            plane.Normal = edgeNormal.Perpendicular();
            plane.Depth = Vector2.Dot(plane.Normal, polyLine.Startpoint);
            return true;
        }

        private bool Clip(Plane plane)
        {
            var output = new Vector2[2];
            var index = 0;
            var distanceA = plane.Distance(Startpoint);
            var distanceB = plane.Distance(Endpoint);

            if (distanceA < 0f) output[index++] = Startpoint;
            if (distanceB < 0f) output[index++] = Endpoint;

            if (distanceA * distanceB < 0f)
                output[index++] = Startpoint + Edge * (distanceA / (distanceA - distanceB));

            Startpoint = output[0];
            Endpoint = output[1];

            return index == 2;
        }
    }

    internal struct Plane
    {
        public Vector2 Normal { get; set; }

        public float Depth { get; set; }

        public Plane(Vector2 normal, float depth)
        {
            Normal = normal;
            Depth = depth;
        }

        public float Distance(Vector2 point)
        {
            return Vector2.Dot(Normal, point) - Depth;
        }

        public Vector2 Project(Vector2 point)
        {
            return point - Normal * Distance(point);
        }
    }
}
