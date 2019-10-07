using System.Collections;
using System.Collections.Generic;

namespace FrogWorks
{
    public abstract class Manager<TItem, TContainer> : IManager<TItem>
        where TItem : Managable<TContainer>
        where TContainer : class
    {
        protected TContainer Container { get; private set; }

        protected List<TItem> Items { get; private set; }

        protected List<TItem> ToAdd { get; private set; }

        protected List<TItem> ToRemove { get; private set; }

        public TItem this[int index] => Items[index];

        public int Count => Items.Count;

        protected Manager(TContainer container)
        {
            Container = container;
            Items = new List<TItem>();
            ToAdd = new List<TItem>();
            ToRemove = new List<TItem>();
        }

        internal void ProcessQueues()
        {
            if (ToRemove.Count > 0)
            {
                foreach (var item in ToRemove)
                    TryRemove(item);
                ToRemove.Clear();
            }

            if (ToAdd.Count > 0)
            {
                foreach (var item in ToAdd)
                    TryAdd(item);
                ToAdd.Clear();
            }

            PostProcessQueues();
        }

        protected virtual void PostProcessQueues() { }

        internal virtual void Update(float deltaTime)
        {
            foreach (var item in Items)
                if (item.IsEnabled)
                    item.InternalUpdate(deltaTime);
        }

        internal virtual void Draw(RendererBatch batch)
        {
            foreach (var item in Items)
                if (item.IsVisible)
                    item.InternalDraw(batch);
        }

        public void Add(TItem item)
        {
            if (Items.Contains(item) && ToRemove.Contains(item))
                ToRemove.Remove(item);
            else if (!ToAdd.Contains(item))
                ToAdd.Add(item);
        }

        public void Add(params TItem[] items)
        {
            foreach (var item in items)
                Add(item);
        }

        public void Add(IEnumerable<TItem> items)
        {
            foreach (var item in items)
                Add(item);
        }

        public void Remove(TItem item)
        {
            if (!Items.Contains(item) && ToAdd.Contains(item))
                ToAdd.Remove(item);
            else if (!ToRemove.Contains(item))
                ToRemove.Add(item);
        }

        public void Remove(params TItem[] items)
        {
            foreach (var item in items)
                Remove(item);
        }

        public void Remove(IEnumerable<TItem> items)
        {
            foreach (var item in items)
                Remove(item);
        }

        protected void TryAdd(TItem item)
        {
            if (!Items.Contains(item))
            {
                Items.Add(item);
                item.OnInternalAdded(Container);
            }
        }

        protected void TryRemove(TItem item)
        {
            if (Items.Contains(item))
            {
                Items.Remove(item);
                item.OnInternalRemoved();
            }
        }

        public TItem[] ToArray()
        {
            return Items.ToArray();
        }

        public IEnumerator<TItem> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

