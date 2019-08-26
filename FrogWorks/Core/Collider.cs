using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public abstract class Collider
    {
        protected internal Entity Parent { get; private set; }

        protected internal Layer Layer => Parent?.Parent;

        protected internal Scene Scene => Parent?.Scene;

        protected internal bool IsCollidable => Parent?.IsCollidable ?? true;

        public Vector2 Position { get; set; }

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

        public virtual void DebugDraw(RendererBatch batch, Color color, bool fill = false) { }

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

        internal void OnInternalEntityAdded() => OnEntityAdded();

        internal void OnInternalEntityRemoved() => OnEntityRemoved();

        protected virtual void OnEntityAdded() { }

        protected virtual void OnEntityRemoved() { }
    }
}
