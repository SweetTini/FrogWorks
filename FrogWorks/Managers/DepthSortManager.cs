using System.Collections.Generic;

namespace FrogWorks
{
    public abstract class DepthSortManager<T, TT> : Manager<T, TT>
        where T : Managable<TT> where TT : class
    {
        protected Queue<SortedManagedQueueCommand<T, TT>> QueuedMovedItems { get; private set; }

        protected DepthSortManager(TT container)
            : base(container)
        {
            QueuedMovedItems = new Queue<SortedManagedQueueCommand<T, TT>>();
        }

        protected override void PostProcessQueues()
        {
            while (QueuedMovedItems.Count > 0)
                TryMove(QueuedMovedItems.Dequeue());
        }

        public void MoveToTop(T item)
        {
            if (!Items.Contains(item))
                return;

            var command = new SortedManagedQueueCommand<T, TT>(item, SortedManagedQueueAction.MoveToTop);

            if (!QueuedMovedItems.Contains(command))
                QueuedMovedItems.Enqueue(command);
        }

        public void MoveToBottom(T item)
        {
            if (!Items.Contains(item))
                return;

            var command = new SortedManagedQueueCommand<T, TT>(item, SortedManagedQueueAction.MoveToBottom);

            if (!QueuedMovedItems.Contains(command))
                QueuedMovedItems.Enqueue(command);
        }

        public void MoveAbove(T item, T other)
        {
            if (item.Equals(other) || !Items.Contains(item) || !Items.Contains(other))
                return;

            var command = new SortedManagedQueueCommand<T, TT>(item, other, SortedManagedQueueAction.MoveAbove);

            if (!QueuedMovedItems.Contains(command))
                QueuedMovedItems.Enqueue(command);
        }

        public void MoveBelow(T item, T other)
        {
            if (item.Equals(other) || !Items.Contains(item) || !Items.Contains(other))
                return;

            var command = new SortedManagedQueueCommand<T, TT>(item, other, SortedManagedQueueAction.MoveBelow);

            if (!QueuedMovedItems.Contains(command))
                QueuedMovedItems.Enqueue(command);
        }

        protected void TryMove(SortedManagedQueueCommand<T, TT> command)
        {
            switch (command.Action)
            {
                case SortedManagedQueueAction.MoveToTop:
                    if (Items.Contains(command.Item))
                    {
                        Items.Remove(command.Item);
                        Items.Add(command.Item);
                    }
                    break;
                case SortedManagedQueueAction.MoveToBottom:
                    if (Items.Contains(command.Item))
                    {
                        Items.Remove(command.Item);
                        Items.Insert(0, command.Item);
                    }
                    break;
                case SortedManagedQueueAction.MoveAbove:
                    if (Items.Contains(command.Item) && Items.Contains(command.Target))
                    {
                        Items.Remove(command.Item);
                        Items.Insert(Items.IndexOf(command.Target) + 1, command.Item);
                    }
                    break;
                case SortedManagedQueueAction.MoveBelow:
                    if (Items.Contains(command.Item) && Items.Contains(command.Target))
                    {
                        Items.Remove(command.Item);
                        Items.Insert(Items.IndexOf(command.Target), command.Item);
                    }
                    break;
            }
        }
    }

    public struct SortedManagedQueueCommand<T, TT> : IManagedQueueCommand<T, SortedManagedQueueAction>
        where T : Managable<TT> where TT : class
    {
        public T Item { get; }

        public T Target { get; }

        public SortedManagedQueueAction Action { get; }

        internal SortedManagedQueueCommand(T item, SortedManagedQueueAction action)
            : this(item, null, action) { }

        internal SortedManagedQueueCommand(T item, T target, SortedManagedQueueAction action)
            : this()
        {
            Item = item;
            Target = target;
            Action = action;
        }
    }

    public enum SortedManagedQueueAction
    {
        MoveToTop,
        MoveToBottom,
        MoveAbove,
        MoveBelow
    }
}
