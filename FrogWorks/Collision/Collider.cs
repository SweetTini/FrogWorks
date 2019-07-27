using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public abstract class Collider
    {
        private Vector2 _position;

        protected internal Entity Entity { get; private set; }

        internal bool IsCollidable => Entity?.IsCollidable ?? true;

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

        public abstract Vector2 Upper { get; set; }

        public abstract Vector2 Lower { get; set; }

        public float Left
        {
            get { return Upper.X; }
            set { Upper = new Vector2(value, Upper.Y); }
        }

        public float Right
        {
            get { return Lower.X; }
            set { Lower = new Vector2(value, Lower.Y); }
        }

        public float Top
        {
            get { return Upper.Y; }
            set { Upper = new Vector2(Upper.X, value); }
        }

        public float Bottom
        {
            get { return Lower.Y; }
            set { Lower = new Vector2(Lower.X, value); }
        }

        protected Collider()
        {
        }

        public virtual void Draw(RendererBatch batch, Color color)
        {
        }

        public abstract bool Contains(Vector2 point);

        public bool Contains(float x, float y)
        {
            return Contains(new Vector2(x, y));
        }

        public abstract bool CastRay(Ray ray, out Raycast hit);

        public abstract bool Collide(Shape other);

        public abstract bool Collide(Shape other, out Manifold hit);

        public abstract bool Collide(Collider other);

        public abstract bool Collide(Collider other, out Manifold hit);

        public bool Collide(Entity entity)
        {
            return entity?.Collider != null && Collide(entity.Collider);
        }

        public bool Collide(Entity entity, out Manifold hit)
        {
            hit = new Manifold();

            return entity?.Collider != null && Collide(entity.Collider, out hit);
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

        internal virtual void OnEntityAdded(Entity entity)
        {
        }

        internal virtual void OnEntityRemoved(Entity entity)
        {
        }

        internal virtual void OnLayerChanged(Layer layer, Layer lastLayer)
        {
        }

        internal virtual void OnTranslated(Vector2 offset)
        {
        }

        internal virtual void OnTransformed()
        {
        }
    }
}
