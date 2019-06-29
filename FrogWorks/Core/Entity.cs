using Microsoft.Xna.Framework;
using System;

namespace FrogWorks
{
    public abstract class Entity
    {
        private Vector2 _position;
        private int _depth;

        internal static Comparison<Entity> CompareDepth
        {
            get { return (entity, other) => Math.Sign(entity.Depth - other.Depth); }
        }

        public Layer Layer { get; private set; }

        public Scene Scene => Layer?.Scene;

        internal ComponentManager Components { get; private set; }

        public Vector2 Position
        {
            get { return _position; }
            set
            {
                if (value == _position) return;
                var lastPosition = _position;
                _position = value;
                OnTranslated(_position, lastPosition);
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
                OnTranslated(_position, lastPosition);
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
                OnTranslated(_position, lastPosition);
            }
        }

        public int Depth
        {
            get { return _depth; }
            set
            {
                if (value == _depth) return;
                _depth = value;
                Scene?.Entities.MarkUnsorted();
            }
        }

        public bool IsEnabled { get; set; } = true;

        public bool IsVisible { get; set; } = true;

        public bool IsDestroyed { get; internal set; }

        protected Entity()
        {
            Components = new ComponentManager(this);
        }

        public virtual void Update(float deltaTime)
        {
            Components.ProcessQueues();
            Components.Update(deltaTime);
        }

        public virtual void Draw(RendererBatch batch)
        {
            Components.Draw(batch);
        }

        public virtual void OnAdded(Layer layer)
        {
            Layer = layer;

            foreach (var component in Components)
                component.OnEntityAdded(this);
        }

        public virtual void OnRemoved()
        {
            foreach (var component in Components)
                component.OnEntityRemoved(this);

            Layer = null;
        }

        public virtual void OnSceneBegan(Scene scene)
        {
        }

        public virtual void OnSceneEnded(Scene scene)
        {
        }

        public virtual void OnTranslated(Vector2 position, Vector2 lastPosition)
        {
        }

        public void Destroy()
        {
            Scene?.Entities.Remove(this);
            IsDestroyed = true;
        }
    }
}
