namespace FrogWorks
{
    public abstract class Component : Manageable<Entity>
    {
        protected internal Entity Entity => Parent;

        protected internal Scene Scene => Parent?.Parent;

        protected internal Layer Layer => Parent?.Layer;

        protected Component(bool isActive, bool isVisible)
        {
            IsActive = isActive;
            IsVisible = isVisible;
        }

        internal void OnEntityAddedInternally()
        {
            OnEntityAdded();
        }

        internal void OnEntityRemovedInternally()
        {
            OnEntityRemoved();
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

        public sealed override void Destroy()
        {
            Parent?.Components.Remove(this);
        }
    }
}
