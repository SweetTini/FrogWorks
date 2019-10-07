using Microsoft.Xna.Framework;
using System;

namespace FrogWorks
{
    public abstract class Entity : Managable<Layer>
    {
        private Vector2 _position;
        private Collider _collider;

        protected internal Scene Scene => Parent?.Parent;

        public ComponentManager Components { get; private set; }

        public Collider Collider
        {
            get { return _collider; }
            protected set
            {
                if (value == _collider)
                    return;

                if (value?.Parent != null)
                    throw new Exception($"{value.GetType().Name} is already assigned to an instance of {GetType().Name}.");

                _collider?.OnInternalRemoved();
                _collider = value;
                _collider?.OnInternalAdded(this);
            }
        }

        public bool IsCollidable { get; set; } = true;

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
            Components.ProcessQueues();
            Components.Update(deltaTime);
        }

        protected sealed override void Draw(RendererBatch batch) => Components.Draw(batch);

        protected override void OnAdded()
        {
            Components.ProcessQueues();

            foreach (var component in Components)
                component.OnInternalEntityAdded();

            Collider?.OnInternalEntityAdded();
        }

        protected override void OnRemoved()
        {
            Collider?.OnInternalEntityRemoved();

            foreach (var component in Components)
                component.OnInternalEntityRemoved();
        }

        internal void OnInternalLayerAdded()
        {
            OnLayerAdded();

            Collider?.OnInternalLayerAdded();

            foreach (var component in Components)
                component.OnInternalLayerAdded();
        }

        internal void OnInternalLayerRemoved()
        {
            foreach (var component in Components)
                component.OnInternalLayerRemoved();

            Collider?.OnInternalLayerRemoved();

            OnLayerRemoved();
        }

        protected virtual void OnLayerAdded() { }

        protected virtual void OnLayerRemoved() { }

        internal void OnInternalSceneBegan() => OnSceneBegan();

        internal void OnInternalSceneEnded() => OnSceneEnded();

        protected virtual void OnSceneBegan() { }

        protected virtual void OnSceneEnded() { }

        internal void OnInternalTransformed()
        {
            OnTransformed();

            Collider?.OnInternalTransformed();

            foreach (var component in Components)
                component.OnInternalTransformed();
        }

        protected virtual void OnTransformed() { }

        public sealed override void Destroy() => Parent?.Entities.Remove(this);
    }
}
