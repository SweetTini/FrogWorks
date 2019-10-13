using System.Collections.Generic;

namespace FrogWorks
{
    public abstract class DepthSortManager<T, TT> : Manager<T, TT>
        where T : Managable<TT>
        where TT : class
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

            var command = new SortedManagedQueueCommand<T, TT>(item, ManagedQueueAction.MoveToTop);

            if (!QueuedMovedItems.Contains(command))
                QueuedMovedItems.Enqueue(command);
        }

        public void MoveToBottom(T item)
        {
            if (!Items.Contains(item))
                return;

            var command = new SortedManagedQueueCommand<T, TT>(item, ManagedQueueAction.MoveToBottom);

            if (!QueuedMovedItems.Contains(command))
                QueuedMovedItems.Enqueue(command);
        }

        public void MoveAbove(T item, T other)
        {
            if (item.Equals(other) || !Items.Contains(item) || !Items.Contains(other))
                return;

            var command = new SortedManagedQueueCommand<T, TT>(item, other, ManagedQueueAction.MoveAbove);

            if (!QueuedMovedItems.Contains(command))
                QueuedMovedItems.Enqueue(command);
        }

        public void MoveBelow(T item, T other)
        {
            if (item.Equals(other) || !Items.Contains(item) || !Items.Contains(other))
                return;

            var command = new SortedManagedQueueCommand<T, TT>(item, other, ManagedQueueAction.MoveBelow);

            if (!QueuedMovedItems.Contains(command))
                QueuedMovedItems.Enqueue(command);
        }

        protected void TryMove(SortedManagedQueueCommand<T, TT> command)
        {
            switch (command.Action)
            {
                case ManagedQueueAction.MoveToTop:
                    if (Items.Contains(command.Item))
                    {
                        Items.Remove(command.Item);
                        Items.Add(command.Item);
                    }
                    break;
                case ManagedQueueAction.MoveToBottom:
                    if (Items.Contains(command.Item))
                    {
                        Items.Remove(command.Item);
                        Items.Insert(0, command.Item);
                    }
                    break;
                case ManagedQueueAction.MoveAbove:
                    if (Items.Contains(command.Item) && Items.Contains(command.Target))
                    {
                        Items.Remove(command.Item);
                        Items.Insert(Items.IndexOf(command.Target) + 1, command.Item);
                    }
                    break;
                case ManagedQueueAction.MoveBelow:
                    if (Items.Contains(command.Item) && Items.Contains(command.Target))
                    {
                        Items.Remove(command.Item);
                        Items.Insert(Items.IndexOf(command.Target), command.Item);
                    }
                    break;
            }
        }
    }

    public class SortedManagedQueueCommand<T, TT> : ManagedQueueCommand<T, TT>
        where T : Managable<TT>
        where TT : class
    {
        public T Target { get; private set; }

        internal SortedManagedQueueCommand(T item, ManagedQueueAction action)
            : this(item, null, action) { }

        internal SortedManagedQueueCommand(T item, T target, ManagedQueueAction action) 
            :  base(item, action)
        {
            Target = target;
        }
    }
}
