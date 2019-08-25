using Microsoft.Xna.Framework;

namespace FrogWorks
{
    public abstract class Entity : AbstractManageable<Layer>
    {
        private Collider _collider;

        protected internal Scene ParentScene => Parent.Parent;

        protected internal ComponentManager Components { get; private set; }

        protected internal Collider Collider
        {
            get { return _collider; }
            set
            {
                if (value == _collider || value.Parent != null) return;
                _collider?.OnRemoved();
                _collider = value;
                _collider?.OnAdded(this);
            }
        }

        public bool IsCollidable { get; set; } = true;

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

        protected Entity()
        {
            Components = new ComponentManager(this);
        }

        protected sealed override void Update(float deltaTime) => Components.Update(deltaTime);

        protected sealed override void Draw(RendererBatch batch) => Components.Draw(batch);

        protected override void OnAdded()
        {
            foreach (var component in Components)
                component.OnInternalEntityAdded();
        }

        protected override void OnRemoved()
        {
            foreach (var component in Components)
                component.OnInternalEntityRemoved();
        }

        public override void Destroy() => Parent?.Entities.Remove(this);
    }
}
