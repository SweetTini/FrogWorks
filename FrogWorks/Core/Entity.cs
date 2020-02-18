using Microsoft.Xna.Framework;
using System.Collections;
using System.Collections.Generic;

namespace FrogWorks
{
    public abstract class Entity : Manageable<Scene>, IManager<Component>
    {
        Layer _layer;
        Collider _collider;
        Vector2 _position;

        protected Scene Scene => Parent;

        internal ComponentManager Components { get; private set; }

        protected internal Layer Layer
        {
            get { return _layer; }
            internal set
            {
                if (_layer != value)
                {
                    OnRemovedInternally();
                    _layer = value;
                    OnLayerAddedInternally();
                }
            }
        }

        public Collider Collider
        {
            get { return _collider; }
            protected set
            {
                if (_collider != value)
                {
                    _collider?.OnRemovedInternally();
                    _collider = value;
                    _collider?.OnAddedInternally(this);
                }
            }
        }

        public bool IsCollidable { get; set; } = true;

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

        public Vector2 Size
        {
            get { return Collider?.Size ?? Vector2.Zero; }
            set { if (Collider != null) Collider.Size = value; }
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

        public Vector2 Upper
        {
            get { return Collider?.Upper ?? Position; }
            set { Position = value - ((Collider?.Upper ?? Position) - Position); }
        }

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

        public Vector2 Lower
        {
            get { return Collider?.Lower ?? Position; }
            set { Position = value - ((Collider?.Lower ?? Position) - Position); }
        }

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
            get { return Collider?.Center ?? Position; }
            set { Position = value - ((Collider?.Center ?? Position) - Position); }
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

        protected Entity()
        {
            Components = new ComponentManager(this);
        }

        protected sealed override void Update(float deltaTime)
        {
            Components.Update(deltaTime);
        }

        protected sealed override void Draw(RendererBatch batch)
        {
            Components.Draw(batch);
        }

        protected override void OnAdded()
        {
            foreach (var component in Components)
                component.OnEntityAddedInternally();
        }

        protected override void OnRemoved()
        {
            foreach (var component in Components)
                component.OnEntityRemovedInternally();
        }

        internal void OnSceneBeganInternally()
        {
            OnSceneBegan();
        }

        internal void OnSceneEndedInternally()
        {
            OnSceneEnded();
        }

        internal void OnLayerAddedInternally()
        {
            OnLayerAdded();
            Collider?.OnLayerAddedInternally();

            foreach (var component in Components)
                component.OnLayerAddedInternally();
        }

        internal void OnLayerRemovedInternally()
        {
            foreach (var component in Components)
                component.OnLayerRemovedInternally();

            Collider?.OnLayerRemovedInternally();
            OnLayerRemoved();
        }

        internal void OnTransformedInternally()
        {
            OnTransformed();
            Collider?.OnTransformedInternally();

            foreach (var component in Components)
                component.OnTransformedInternally();
        }

        protected virtual void OnSceneBegan()
        {
        }

        protected virtual void OnSceneEnded()
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

        public sealed override void Destroy()
        {
            Parent?.Entities.Remove(this);
        }

        #region Components
        public void Add(Component component)
        {
            Components.Add(component);
        }

        public void Add(params Component[] components)
        {
            Components.Add(components);
        }

        public void Add(IEnumerable<Component> components)
        {
            Components.Add(components);
        }

        public void Remove(Component component)
        {
            Components.Remove(component);
        }

        public void Remove(params Component[] components)
        {
            Components.Remove(components);
        }

        public void Remove(IEnumerable<Component> components)
        {
            Components.Remove(components);
        }

        public IEnumerator<Component> GetEnumerator()
        {
            return Components.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
    }
}
