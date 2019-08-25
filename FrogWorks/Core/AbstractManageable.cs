using System;

namespace FrogWorks
{
    public abstract class AbstractManageable<T>
        where T : class
    {
        protected internal T Parent { get; private set; }

        public bool IsEnabled { get; set; } = true;

        public bool IsVisible { get; set; } = true;

        internal void InternalUpdate(float deltaTime)
        {
            BeforeUpdate(deltaTime);
            Update(deltaTime);
            AfterUpdate(deltaTime);
        }

        protected virtual void Update(float deltaTime) { }

        protected virtual void BeforeUpdate(float deltaTime) { }

        protected virtual void AfterUpdate(float deltaTime) { }

        internal void InternalDraw(RendererBatch batch)
        {
            BeforeDraw(batch);
            Draw(batch);
            AfterDraw(batch);
        }

        protected virtual void Draw(RendererBatch batch) { }

        protected virtual void BeforeDraw(RendererBatch batch) { }

        protected virtual void AfterDraw(RendererBatch batch) { }

        internal void OnInternalAdded(T parent)
        {
            if (Parent != null)
                throw new Exception($"{GetType().Name} already has a {nameof(T)} assigned.");

            Parent = parent;
            OnAdded();
        }

        internal void OnInternalRemoved()
        {
            if (Parent != null)
                OnRemoved();

            Parent = null;
        }

        protected virtual void OnAdded() { }

        protected virtual void OnRemoved() { }

        public abstract void Destroy();
    }
}
