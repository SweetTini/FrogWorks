using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public abstract class Collider
    {
        private Vector2 _position;

        protected internal Entity Entity { get; private set; }

        public Vector2 Position
        {
            get { return _position; }
            set
            {
                if (value == _position) return;
                var lastPosition = _position;
                _position = value;
                OnTranslated(_position - lastPosition);
            }
        }

        public float X
        {
            get { return _position.X; }
            set
            {
                if (value == _position.X) return;
                var lastPosition = _position;
                _position.X = value;
                OnTranslated(_position - lastPosition);
            }
        }

        public float Y
        {
            get { return _position.Y; }
            set
            {
                if (value == _position.Y) return;
                var lastPosition = _position;
                _position.Y = value;
                OnTranslated(_position - lastPosition);
            }
        }

        public Vector2 AbsolutePosition
        {
            get { return Position + (Entity?.Position ?? Vector2.Zero); }
            set { Position = value - (Entity?.Position ?? Vector2.Zero); }
        }

        public float AbsoluteX
        {
            get { return X + (Entity?.X ?? 0f); }
            set { X = value - (Entity?.X ?? 0f); }
        }

        public float AbsoluteY
        {
            get { return Y + (Entity?.Y ?? 0f); }
            set { Y = value - (Entity?.Y ?? 0f); }
        }

        public abstract Vector2 Size { get; set; }

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

        protected Collider()
        {
        }

        public abstract bool Contains(Vector2 point);

        public bool Contains(float x, float y)
        {
            return Contains(new Vector2(x, y));
        }

        public abstract bool CastRay(Ray ray, out Raycast hit);

        public abstract bool Collide(Shape shape);

        public abstract bool Collide(Shape shape, out Manifold hit);

        public virtual void Draw(RendererBatch batch, Color color)
        {
        }

        public abstract Collider Clone();

        internal virtual void OnAdded(Entity entity)
        {
            Entity = entity;
        }

        internal virtual void OnRemoved()
        {
            Entity = null;
        }

        internal virtual void OnTranslated(Vector2 offset)
        {
        }

        internal virtual void OnTransformed()
        {
        }
    }
}
