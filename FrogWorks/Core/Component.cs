namespace FrogWorks
{
    public abstract class Component : AbstractManageable<Entity>
    {
        protected internal Layer ParentLayer => Parent.Parent;

        protected internal Scene ParentScene => Parent.ParentScene;

        protected Component(bool isEnabled, bool isVisible)
        {
            IsEnabled = isEnabled;
            IsVisible = isVisible;
        }

        internal void OnInternalEntityAdded() => OnEntityAdded();

        internal void OnInternalEntityRemoved() => OnEntityRemoved();

        protected virtual void OnEntityAdded() { }

        protected virtual void OnEntityRemoved() { }

        public override void Destroy() => Parent?.Components.Remove(this);
    }
}
