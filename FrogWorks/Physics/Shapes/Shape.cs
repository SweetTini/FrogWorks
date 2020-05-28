using Microsoft.Xna.Framework;
using System;

namespace FrogWorks
{
    public abstract class Shape
    {
        Vector2 _position;
        Rectangle _bounds;
        bool _isDirty = true;

        public Vector2 Position
        {
            get { return _position; }
            set
            {
                if (_position != value)
                {
                    _position = value;
                    _isDirty = true;
                    OnTranslate();
                }
            }
        }

        public float X
        {
            get { return Position.X; }
            set { Position = new Vector2(value, Position.Y); }
        }

        public float Y
        {
            get { return Position.Y; }
            set { Position = new Vector2(Position.X, value); }
        }

        public abstract Vector2 Center { get; set; }

        public float CenterX
        {
            get { return Center.X; }
            set { Center = new Vector2(value, Center.Y); }
        }

        public float CenterY
        {
            get { return Center.Y; }
            set { Center = new Vector2(Center.X, value); }
        }

        public Rectangle Bounds
        {
            get
            {
                RecalculateBoundsInternally();
                return _bounds;
            }
        }

        protected Shape(Vector2 position)
            : base()
        {
            _position = position;
        }

        public abstract bool Contains(Vector2 point);

        public bool Contains(Vector2 point, out Vector2 depth)
        {
            depth = default;

            if (Contains(point))
            {
                depth = GetClosestPointOnPoint(point) - point;
                return true;
            }

            return false;
        }

        public bool CastRay(Vector2 origin, Vector2 normal, float distance)
        {
            return CastRay(origin, normal, distance, out _);
        }

        public bool CastRay(Vector2 origin, Vector2 normal, float distance, out Raycast hit)
        {
            return Collision.CastRay(origin, normal, distance, this, out hit);
        }

        public bool Overlaps(Shape shape)
        {
            return Collision.Overlaps(this, shape);
        }

        public bool Overlaps(Shape shape, out Manifold hit)
        {
            return Collision.Overlaps(this, shape, out hit);
        }

        public virtual Vector2 GetClosestPointOnPoint(Vector2 point)
        {
            var vertices = GetVertices();
            var minDistance = float.PositiveInfinity;
            var closest = Vector2.Zero;

            for (int i = 0; i < vertices.Length; i++)
            {
                var start = vertices[i];
                var end = vertices[(i + 1).Mod(vertices.Length)];
                var next = PlotEX.GetClosestPointOnLine(start, end, point);
                var distance = (point - next).LengthSquared();

                if (minDistance > distance)
                {
                    minDistance = distance;
                    closest = next;
                }
            }

            return closest;
        }

        public abstract Vector2[] GetVertices();

        public void Draw(RendererBatch batch, Color color)
        {
            Draw(batch, color, default);
        }

        public abstract void Draw(RendererBatch batch, Color color, Color fill);

        public abstract Shape Clone();

        internal void RecalculateBoundsInternally()
        {
            if (_isDirty)
            {
                _bounds = RecalculateBounds();
                _isDirty = false;
            }
        }

        protected abstract Rectangle RecalculateBounds();

        protected virtual void OnTranslate()
        {
        }

        protected void MarkAsDirty()
        {
            _isDirty = true;
        }
    }
}
