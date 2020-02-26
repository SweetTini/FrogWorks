using System;

namespace FrogWorks
{
    public class CollidableComponent : Component
    {
        private Collider _collider;

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

        public CollidableComponent(bool isEnabled, bool isVisible, bool isCollidable)
            : base(isEnabled, isVisible)
        {
            IsCollidable = isCollidable;
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
    }
}
