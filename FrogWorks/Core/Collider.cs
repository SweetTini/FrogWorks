using Microsoft.Xna.Framework;
using System;

namespace FrogWorks
{
    public abstract class Collider
    {
        private Vector2 _position;

        internal CollidableComponent Component { get; private set; }

        protected internal Entity Parent { get; private set; }

        protected internal Layer Layer => Parent?.Parent;

        protected internal Scene Scene => Parent?.Scene;

        protected internal bool IsCollidable => Component?.IsCollidable ?? Parent?.IsCollidable ?? true;

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

        public float AbsoluteX
        {
            get { return AbsolutePosition.X; }
            set { AbsolutePosition = new Vector2(value, AbsolutePosition.Y); }
        }

        public float AbsoluteY
        {
            get { return AbsolutePosition.Y; }
            set { AbsolutePosition = new Vector2(AbsolutePosition.X, value); }
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

        public float Left
        {
            get { return Upper.X; }
            set { Upper = new Vector2(value, Upper.Y); }
        }

        public float Top
        {
            get { return Upper.Y; }
            set { Upper = new Vector2(Upper.X, value); }
        }

        public abstract Vector2 Lower { get; set; }

        public float Right
        {
            get { return Lower.X; }
            set { Lower = new Vector2(value, Lower.Y); }
        }

        public float Bottom
        {
            get { return Lower.Y; }
            set { Lower = new Vector2(Lower.X, value); }
        }

        public Vector2 Center
        {
            get { return (Upper + Lower) / 2f; }
            set { Upper = value - (Upper - Lower) / 2f; }
        }

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
            => new Rectangle(Upper.ToPoint(), (Lower - Upper).ToPoint());

        protected Collider(Vector2 position)
        {
            _position = position;
        }

        public virtual void Draw(RendererBatch batch, Color color, bool fill = false) { }

        public abstract bool Collide(Vector2 point);

        public bool Collide(float x, float y) => Collide(new Vector2(x, y));

        public abstract bool Collide(Ray ray);

        public abstract bool Collide(Shape shape);

        public abstract bool Collide(Collider collider);

        public bool Collide(Entity entity) => Collide(entity?.Collider);

        public abstract Collider Clone();

        internal void OnInternalAdded(Entity parent)
        {
            if (Parent != null)
                throw new Exception($"{GetType().Name} is already assigned to an instance of {Parent.GetType().Name}.");

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
            Component = component;
            OnInternalAdded(Component.Parent);
        }

        internal void OnRemovedAsComponent()
        {
            OnInternalRemoved();
            Component = null;
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
