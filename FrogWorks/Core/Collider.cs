using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public abstract class Collider
    {
        private Vector2 _position;

        internal CollidableComponent ParentComponent { get; private set; }

        protected internal Entity Parent { get; private set; }

        protected internal Layer Layer => Parent?.Parent;

        protected internal Scene Scene => Parent?.Scene;

        protected internal bool IsCollidable => ParentComponent?.IsCollidable ?? Parent?.IsCollidable ?? true;

        public Vector2 Position
        {
            get { return _position; }
            set
            {
                if (value == _position) return;
                _position = value;
                OnInternalTransformed();
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

        public Vector2 AbsolutePosition
        {
            get { return Position + (Parent?.Position ?? Vector2.Zero); }
            set { Position = value - (Parent?.Position ?? Vector2.Zero); }
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

        public Vector2 Center
        {
            get { return (Upper + Lower) / 2f; }
            set { Upper = value - (Upper - Lower) / 2f; }
        }

        protected Collider(Vector2 position)
        {
            _position = position;
        }

        public virtual void DebugDraw(RendererBatch batch, Color color, bool fill = false) { }

        public abstract bool Contains(Vector2 point);

        public bool Contains(float x, float y) => Contains(new Vector2(x, y));

        public abstract bool Collide(Ray ray);

        public abstract bool Collide(Shape shape);

        public abstract bool Collide(Collider other);

        public bool Collide(Entity entity) => Collide(entity?.Collider);

        public abstract Collider Clone();

        internal void OnInternalAdded(Entity parent)
        {
            Parent = parent;
            OnAdded();
        }

        internal void OnInternalRemoved()
        {
            OnRemoved();
            Parent = null;
        }

        protected virtual void OnAdded() { }

        protected virtual void OnRemoved() { }

        internal void OnAddedAsComponent(CollidableComponent component)
        {
            ParentComponent = component;
        }

        internal void OnRemovedAsComponent()
        {
            ParentComponent = null;
        }

        internal void OnInternalEntityAdded() => OnEntityAdded();

        internal void OnInternalEntityRemoved() => OnEntityRemoved();

        protected virtual void OnEntityAdded() { }

        protected virtual void OnEntityRemoved() { }

        internal void OnInternalLayerAdded() => OnLayerAdded();

        internal void OnInternalLayerRemoved() => OnLayerRemoved();

        protected virtual void OnLayerAdded() { }

        protected virtual void OnLayerRemoved() { }

        internal void OnInternalTransformed() => OnTransformed();

        protected virtual void OnTransformed() { }
    }
}
