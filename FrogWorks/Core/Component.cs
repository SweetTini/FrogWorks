namespace FrogWorks
{
    public abstract class Component : Manageable<Entity>
    {
        protected internal Layer Layer => Parent?.Parent;

        protected internal Scene Scene => Parent?.Scene;

        protected Component(bool isEnabled, bool isVisible)
        {
            IsEnabled = isEnabled;
            IsVisible = isVisible;
        }

        internal void OnInternalEntityAdded() => OnEntityAdded();

        internal void OnInternalEntityRemoved() => OnEntityRemoved();

        protected virtual void OnEntityAdded() { }

        protected virtual void OnEntityRemoved() { }

        internal void OnInternalLayerAdded() => OnLayerAdded();

        internal void OnInternalLayerRemoved() => OnLayerRemoved();

        protected virtual void OnLayerAdded() { }

        protected virtual void OnLayerRemoved() { }

        internal void OnInternalTransformed() => OnTransformed();

        protected virtual void OnTransformed() { }

        public sealed override void Destroy() => Parent?.Components.Remove(this);
    }
}
