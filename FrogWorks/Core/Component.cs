﻿namespace FrogWorks
{
    public abstract class Component
    {
        protected internal Entity Entity { get; private set; }

        protected internal Layer Layer => Entity?.Layer;

        protected internal Scene Scene => Entity?.Layer?.Scene;

        public bool IsEnabled { get; set; }

        public bool IsVisible { get; set; }

        public bool IsDestroyed { get; internal set; }

        protected Component()
            : this(true, true)
        {
        }

        protected Component(bool isEnabled, bool isVisible)
        {
            IsEnabled = isEnabled;
            IsVisible = isVisible;
        }

        public virtual void Update(float deltaTime)
        {
        }

        public virtual void Draw(RendererBatch batch)
        {
        }

        public virtual void OnAdded(Entity entity)
        {
            Entity = entity;
        }

        public virtual void OnRemoved()
        {
            Entity = null;
        }

        public virtual void OnEntityAdded(Entity entity)
        {
        }

        public virtual void OnEntityRemoved(Entity entity)
        {
        }

        public void Destroy()
        {
            Entity?.Components.Remove(this);
            IsDestroyed = true;
        }
    }
}
