using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public abstract class Collider
    {
        Vector2 _position;

        internal CollidableComponent Component { get; private set; }

        protected internal Entity Entity { get; private set; }

        protected internal Scene Scene => Entity?.Parent;

        protected internal Layer Layer => Entity?.Layer;

        protected internal bool IsCollidable
        {
            get
            {
                return Component?.IsCollidable
                    ?? Entity?.IsCollidable
                    ?? true;
            }
        }

        public Vector2 Position
        {
            get { return _position; }
            set
            {
                if (_position != value)
                {
                    _position = value;
                    OnTransformedInternally();
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

        public Vector2 AbsolutePosition
        {
            get { return Position + (Entity?.Position ?? Vector2.Zero); }
            set { Position = value - (Entity?.Position ?? Vector2.Zero); }
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
            get { return (Upper + Lower) * .5f; }
            set { Upper = value - (Upper - Lower) * .5f; }
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
        {
            get
            {
                return new Rectangle(
                    Upper.ToPoint(),
                    (Lower - Upper).ToPoint());
            }
        }

        protected Collider(Vector2 position)
        {
            _position = position;
        }

        public virtual void Draw(RendererBatch batch, Color stroke, Color? fill = null)
        {
        }

        public abstract bool Collide(Vector2 point);

        public bool Collide(float x, float y)
        {
            return Collide(new Vector2(x, y));
        }

        public abstract bool Collide(Ray2D ray);

        public abstract bool Collide(Shape shape);

        public abstract bool Collide(Collider collider);

        public bool Collide(Entity entity)
        {
            return entity?.Collider != null
                ? Collide(entity.Collider)
                : false;
        }

        public abstract Collider Clone();

        internal void OnAddedAsComponent(CollidableComponent component)
        {
            Component = component;
            OnAddedInternally(Component.Parent);
        }

        internal void OnRemovedAsComponent()
        {
            OnRemovedInternally();
            Component = null;
        }

        internal void OnAddedInternally(Entity entity)
        {
            Entity = entity;
            OnAdded();
        }

        internal void OnRemovedInternally()
        {
            OnRemoved();
            Entity = null;
        }

        internal void OnEntityAddedInternally()
        {
            OnEntityAdded();
        }

        internal void OnEntityRemovedInternally()
        {
            OnEntityRemoved();
        }

        internal void OnLayerAddedInternally()
        {
            OnLayerAdded();
        }

        internal void OnLayerRemovedInternally()
        {
            OnLayerRemoved();
        }

        internal void OnTransformedInternally()
        {
            OnTransformed();
        }

        protected virtual void OnAdded() { }

        protected virtual void OnRemoved() { }

        protected virtual void OnEntityAdded() { }

        protected virtual void OnEntityRemoved() { }

        protected virtual void OnLayerAdded() { }

        protected virtual void OnLayerRemoved() { }

        protected virtual void OnTransformed() { }
    }
}
