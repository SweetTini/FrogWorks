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
                    MarkAsDirty();
                    OnTranslated();
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
                if (_isDirty)
                    RecalculateBoundsInternally();

                return _bounds;
            }
        }

        protected Shape(Vector2 position)
        {
            _position = position;
        }

        internal void RecalculateBoundsInternally()
        {
            _bounds = RecalculateBounds();
        }

        protected abstract Rectangle RecalculateBounds();

        public bool Contains(float x, float y)
        {
            return Contains(new Vector2(x, y));
        }

        public abstract bool Contains(Vector2 point);

        public bool Raycast(float x1, float y1, float x2, float y2, out Raycast hit)
        {
            return Raycast(new Vector2(x1, y1), new Vector2(x2, y2), out hit);
        }

        public bool Raycast(Vector2 start, Vector2 end, out Raycast hit)
        {
            return Collision.Raycast(start, end, this, out hit);
        }

        public bool Overlaps(Shape shape)
        {
            return Collision.Overlaps(this, shape);
        }

        public bool Overlaps(Shape shape, out Manifold hit)
        {
            return Collision.Overlaps(this, shape, out hit);
        }

        public abstract void Draw(
            RendererBatch batch,
            Color strokeColor,
            Color? fillColor = null);

        public abstract Shape Clone();

        protected void MarkAsDirty()
        {
            _isDirty = true;
        }

        protected virtual void OnTranslated()
        {
        }

        internal virtual Vector2[] GetVertices()
        {
            return null;
        }

        internal virtual Vector2[] GetFocis()
        {
            return null;
        }

        internal virtual Vector2[] GetAxes(Vector2[] focis)
        {
            var normals = GetVertices().Normalize();
            var fociCount = focis?.Length ?? 0;
            var axes = new Vector2[normals.Length + fociCount];

            Array.Copy(normals, 0, axes, 0, normals.Length);

            for (int i = 0; i < fociCount; i++)
            {
                var foci = focis[i];
                var offset = normals.Length + i;
                var closest = GetClosestPoint(foci);

                axes[offset] = Vector2.Normalize(foci - closest);
            }

            return axes;
        }

        internal virtual void Project(Vector2 axis, out float min, out float max)
        {
            min = float.PositiveInfinity;
            max = float.NegativeInfinity;

            var vertices = GetVertices();
            float dotProd;

            for (int i = 0; i < vertices.Length; i++)
            {
                dotProd = Vector2.Dot(vertices[i], axis);

                if (min > dotProd) min = dotProd;
                if (max < dotProd) max = dotProd;
            }
        }
        Vector2 GetClosestPoint(Vector2 point)
        {
            var vertices = GetVertices();
            var minDistSq = float.PositiveInfinity;
            var closest = Vector2.Zero;

            for (int i = 0; i < vertices.Length; i++)
            {
                var next = vertices[i];
                var distSq = (point - next).LengthSquared();

                if (minDistSq > distSq)
                {
                    minDistSq = distSq;
                    closest = next;
                }
            }

            return closest;
        }

        #region Static Methods
        internal static float GetIntervalDepth(
            float minA,
            float maxA,
            float minB,
            float maxB)
        {
            if (!(minA > maxB || minB > maxA))
            {
                var min = Math.Min(maxA, maxB);
                var max = Math.Max(minA, minB);
                return min - max;
            }

            return 0f;
        }

        internal static bool IsIntervalContained(
            float minA,
            float maxA,
            float minB,
            float maxB)
        {
            return minA > minB && maxA < maxB;
        }
        #endregion
    }
}
