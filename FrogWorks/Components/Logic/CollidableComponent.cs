using System;

namespace FrogWorks
{
    public class CollidableComponent : Component
    {
        Collider _collider;

        public Collider Collider
        {
            get { return _collider; }
            set
            {
                if (_collider != value)
                {
                    if (value?.Component != null)
                    {
                        throw new Exception($"{value.GetType().Name} is already assigned "
                            + $"to an instance of {GetType().Name}.");
                    }

                    _collider?.OnRemovedAsComponent();
                    _collider = value;
                    _collider?.OnAddedAsComponent(this);
                }
            }
        }

        public bool IsCollidable { get; set; }

        public CollidableComponent(bool isCollidable = true)
            : base(false, true)
        {
            IsCollidable = isCollidable;
        }

        public CollidableComponent(Collider collider, bool isCollidable = true)
            : this(isCollidable)
        {
            Collider = collider;
        }

        protected override void OnAdded()
        {
            if (Entity != null)
                Collider?.OnAddedInternally(Parent);
        }

        protected override void OnRemoved()
        {
            Collider?.OnRemovedInternally();
        }

        protected override void OnTransformed()
        {
            Collider?.OnTransformedInternally();
        }
    }
}
