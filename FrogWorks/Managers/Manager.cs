using System;
using System.Collections;
using System.Collections.Generic;

namespace FrogWorks
{
    public abstract class Manager<T, TT> : IManager<T>
        where T : Managable<TT> where TT : class
    {
        private ManagerState _state;

        protected TT Container { get; private set; }

        protected List<T> Items { get; private set; }

        protected Queue<ManagedQueueCommand<T, TT>> Commands { get; private set; }

        protected ManagerState State
        {
            get { return _state; }
            set
            {
                _state = value;
                ProcessQueues();
            }
        }

        public T this[int index] => Items[index];

        public int Count => Items.Count;

        protected Manager(TT container)
        {
            Container = container;
            Items = new List<T>();
            Commands = new Queue<ManagedQueueCommand<T, TT>>();
        }

        private void ProcessQueues()
        {
            while (Commands.Count > 0)
            {
                var command = Commands.Dequeue();

                switch (command.Action)
                {
                    case ManagedQueueAction.Add: TryAdd(command.Item); break;
                    case ManagedQueueAction.Remove: TryRemove(command.Item); break;
                    default: break;
                }
            }

            PostProcessQueues();
        }

        protected virtual void PostProcessQueues() { }

        internal virtual void Update(float deltaTime)
        {
            State = ManagerState.Queue;

            foreach (var item in Items)
                if (item.IsEnabled)
                    item.InternalUpdate(deltaTime);

            State = ManagerState.Opened;
        }

        internal virtual void Draw(RendererBatch batch)
        {
            State = ManagerState.ThrowError;

            foreach (var item in Items)
                if (item.IsVisible)
                    item.InternalDraw(batch);

            State = ManagerState.Opened;
        }

        public void Add(T item)
        {
            switch (State)
            {
                case ManagerState.Opened:
                    TryAdd(item);
                    break;
                case ManagerState.Queue:
                    Commands.Enqueue(new ManagedQueueCommand<T, TT>(item, ManagedQueueAction.Add));
                    break;
                case ManagerState.ThrowError:
                    throw new Exception($"Cannot add {typeof(T).Name} at this time.");
            }
        }

        public void Add(params T[] items)
        {
            foreach (var item in items)
                Add(item);
        }

        public void Add(IEnumerable<T> items)
        {
            foreach (var item in items)
                Add(item);
        }

        public void Remove(T item)
        {
            switch (State)
            {
                case ManagerState.Opened:
                    TryRemove(item);
                    break;
                case ManagerState.Queue:
                    Commands.Enqueue(new ManagedQueueCommand<T, TT>(item, ManagedQueueAction.Remove));
                    break;
                case ManagerState.ThrowError:
                    throw new Exception($"Cannot remove {typeof(T).Name} at this time.");
            }
        }

        public void Remove(params T[] items)
        {
            foreach (var item in items)
                Remove(item);
        }

        public void Remove(IEnumerable<T> items)
        {
            foreach (var item in items)
                Remove(item);
        }

        protected void TryAdd(T item)
        {
            if (!Items.Contains(item))
            {
                Items.Add(item);
                item.OnInternalAdded(Container);
            }
        }

        protected void TryRemove(T item)
        {
            if (Items.Contains(item))
            {
                Items.Remove(item);
                item.OnInternalRemoved();
            }
        }

        public T[] ToArray()
        {
            return Items.ToArray();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public struct ManagedQueueCommand<T, TT> : IManagedQueueCommand<T, ManagedQueueAction>
        where T : Managable<TT> where TT : class
    {
        public T Item { get; }

        public ManagedQueueAction Action { get; }

        internal ManagedQueueCommand(T item, ManagedQueueAction action)
            : this()
        {
            Item = item;
            Action = action;
        }
    }

    public enum ManagedQueueAction
    {
        Add,
        Remove
    }

    public enum ManagerState
    {
        Opened,
        Queue,
        ThrowError
    }
}

