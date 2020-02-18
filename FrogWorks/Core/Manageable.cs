using System.Collections;
using System.Collections.Generic;

namespace FrogWorks
{
    public abstract class Manageable<T>
        where T : class
    {
        internal T Parent { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsVisible { get; set; } = true;

        internal virtual void UpdateInternally(float deltaTime)
        {
            BeforeUpdate(deltaTime);
            Update(deltaTime);
            AfterUpdate(deltaTime);
        }

        internal virtual void DrawInternally(RendererBatch batch)
        {
            BeforeDraw(batch);
            Draw(batch);
            AfterDraw(batch);
        }

        internal void OnAddedInternally(T instance)
        {
            Parent = instance;
            OnAdded();
        }

        internal void OnRemovedInternally()
        {
            if (Parent != null)
                OnRemoved();

            Parent = null;
        }

        protected virtual void Update(float deltaTime) { }

        protected virtual void BeforeUpdate(float deltaTime) { }

        protected virtual void AfterUpdate(float deltaTime) { }

        protected virtual void Draw(RendererBatch batch) { }

        protected virtual void BeforeDraw(RendererBatch batch) { }

        protected virtual void AfterDraw(RendererBatch batch) { }

        protected virtual void OnAdded() { }

        protected virtual void OnRemoved() { }

        public abstract void Destroy();
    }

    public interface IManager<C> : IEnumerable<C>, IEnumerable
    {
        void Add(C child);

        void Add(params C[] children);

        void Add(IEnumerable<C> children);

        void Remove(C child);

        void Remove(params C[] children);

        void Remove(IEnumerable<C> children);
    }
}
