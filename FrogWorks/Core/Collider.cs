﻿using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public abstract class Collider
    {
        Vector2 _position;

        internal CollidableComponent Component { get; private set; }

        protected internal Entity Entity { get; private set; }

        protected internal Scene Scene => Component?.Scene ?? Entity?.Parent;

        protected internal Layer Layer => Component?.Layer ?? Entity?.Layer;

        protected internal bool IsCollidable
        {
            get { return Component?.IsCollidable ?? Entity?.IsCollidable ?? true; }
        }

        public Rectangle Bounds
        {
            get
            {
                var location = Min.ToPoint();
                var size = (Max - Min).ToPoint();
                return new Rectangle(location, size);
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

        public abstract Vector2 Min { get; set; }

        public float Left
        {
            get { return Min.X; }
            set { Min = new Vector2(value, Min.Y); }
        }

        public float Top
        {
            get { return Min.Y; }
            set { Min = new Vector2(Min.X, value); }
        }

        public abstract Vector2 Max { get; set; }

        public float Right
        {
            get { return Max.X; }
            set { Max = new Vector2(value, Max.Y); }
        }

        public float Bottom
        {
            get { return Max.Y; }
            set { Max = new Vector2(Max.X, value); }
        }

        public Vector2 Center
        {
            get { return (Min + Max) * .5f; }
            set { Min = value - (Min - Max) * .5f; }
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

        protected Collider(Vector2 position)
        {
            _position = position;
        }

        public bool Contains(float x, float y)
        {
            return Contains(new Vector2(x, y));
        }

        public abstract bool Contains(Vector2 point);

        public bool Raycast(float x1, float y1, float x2, float y2, out Raycast hit)
        {
            return Raycast(new Vector2(x1, y1), new Vector2(x2, y2), out hit);
        }

        public abstract bool Raycast(Vector2 start, Vector2 end, out Raycast hit);

        public abstract bool Overlaps(Shape shape);

        public abstract bool Overlaps(Shape shape, out Manifold hit);

        public abstract void Draw(RendererBatch batch, Color color);

        public abstract Collider Clone();

        internal AABB CreateAABB()
        {
            return new AABB(Min, Max);
        }

        internal void OnAddedAsComponent(CollidableComponent component)
        {
            Component = component;
            OnAddedInternally(component?.Entity);
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
            OnTransformedInternally();
            Scene?.World.AddCollider(this);
        }

        internal void OnRemovedInternally()
        {
            OnRemoved();
            OnTransformedInternally();
            Scene?.World.RemoveCollider(this);
            
            Entity = null;
        }

        internal void OnEntityAddedInternally()
        {
            OnEntityAdded();
            OnTransformedInternally();
            Scene?.World.AddCollider(this);
        }

        internal void OnEntityRemovedInternally()
        {
            OnEntityRemoved();
            OnTransformedInternally();
            Scene?.World.RemoveCollider(this);
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
            Scene?.World.UpdateCollider(this);
        }

        protected virtual void OnAdded()
        {
        }

        protected virtual void OnRemoved()
        {
        }

        protected virtual void OnEntityAdded()
        {
        }

        protected virtual void OnEntityRemoved()
        {
        }

        protected virtual void OnLayerAdded()
        {
        }

        protected virtual void OnLayerRemoved()
        {
        }

        protected virtual void OnTransformed()
        {
        }
    }
}
