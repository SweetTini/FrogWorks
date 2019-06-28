using Microsoft.Xna.Framework;
using System;

namespace FrogWorks
{
    public abstract class Entity
    {
        private Vector2 _position;

        internal static Comparison<Entity> Compare
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

        public int Depth { get; set; }

        public bool IsEnabled { get; set; } = true;

        public bool IsVisible { get; set; } = true;

        protected Entity()
        {
        }

        public virtual void Update(float deltaTime)
        {
        }

        public virtual void Draw(RendererBatch batch)
        {
        }

        public virtual void OnAdded(Layer layer)
        {
            Layer = layer;
        }

        public virtual void OnRemoved()
        {
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
        }
    }
}
