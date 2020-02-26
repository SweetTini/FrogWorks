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

        protected abstract bool Contains(Vector2 point);

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

        internal abstract Vector2[] GetAxes();

        internal abstract void Project(Vector2 axis, out float min, out float max);

        internal abstract Vector2 GetClosestPoint(Vector2 point);

        #region Static Methods
        internal static Vector2 GetClosestPointOnLine(
            Vector2 p1, 
            Vector2 p2, 
            Vector2 point)
        {
            var u = p2 - p1;
            var v = point - p1;
            var t = Vector2.Dot(v, u) / Vector2.Dot(u, u);
            return p1 + u * t.Clamp(0f, 1f);
        }

        internal static float GetIntervalDepth(
            float minA, 
            float maxA, 
            float minB, 
            float maxB)
        {
            if (minA > maxB || minB > maxA)
            {
                var min = Math.Min(maxA, maxB);
                var max = Math.Max(minA, minB);
                return min - max;
            }

            return 0f;
        }
        #endregion
    }
}
