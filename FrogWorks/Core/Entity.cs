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
        }

        protected virtual void OnTransformed() { }

        public override void Destroy() => Parent?.Entities.Remove(this);
    }
}
