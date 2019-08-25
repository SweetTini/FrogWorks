using System.Collections;
using System.Collections.Generic;

namespace FrogWorks
{
    public abstract class AbstractManager<TItem, TContainer> : IManager<TItem>
        where TItem : AbstractManageable<TContainer>
        where TContainer : class
    {
        protected TContainer Container { get; private set; }

        protected List<TItem> Items { get; private set; }

        protected Queue<TItem> ToAdd { get; private set; }

        protected Queue<TItem> ToRemove { get; private set; }

        protected bool IsBusy { get; set; }

        public TItem this[int index] => Items[index];

        public int Count => Items.Count;

        protected AbstractManager(TContainer container)
        {
            Container = container;
            Items = new List<TItem>();
            ToAdd = new Queue<TItem>();
            ToRemove = new Queue<TItem>();
        }

        internal void ProcessQueues()
        {
            while (ToAdd.Count > 0)
                TryAdd(ToAdd.Dequeue());

            while (ToRemove.Count > 0)
                TryRemove(ToRemove.Dequeue());

            PostProcessQueues();
        }

        protected virtual void PostProcessQueues() { }

        internal virtual void Update(float deltaTime)
        {
            IsBusy = true;

            foreach (var item in Items)
                if (item.IsEnabled)
                    item.InternalUpdate(deltaTime);

            IsBusy = false;
        }

        internal virtual void Draw(RendererBatch batch)
        {
            IsBusy = true;

            foreach (var item in Items)
                if (item.IsVisible)
                    item.InternalDraw(batch);

            IsBusy = false;
        }

        public void Add(TItem item)
        {
            if (Items.Contains(item))
                return;

            if (IsBusy && !ToAdd.Contains(item))
                ToAdd.Enqueue(item);
            else if (!IsBusy)
                TryAdd(item);
        }

        public void Add(IEnumerable<TItem> items)
        {
            foreach (var item in items)
                Add(item);
        }

        public void Remove(TItem item)
        {
            if (!Items.Contains(item))
                return;

            if (IsBusy && !ToRemove.Contains(item))
                ToRemove.Enqueue(item);
            else if (!IsBusy)
                TryRemove(item);
        }

        public void Remove(IEnumerable<TItem> items)
        {
            foreach (var item in items)
                Remove(item);
        }

        protected void TryAdd(TItem item)
        {
            Items.Add(item);
            item.OnInternalAdded(Container);
        }

        protected void TryRemove(TItem item)
        {
            Items.Remove(item);
            item.OnInternalRemoved();
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

