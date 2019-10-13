using System.Collections.Generic;

namespace FrogWorks
{
    public abstract class DepthSortManager<T, TT> : Manager<T, TT>
        where T : Managable<TT> where TT : class
    {
        protected Queue<ManagedQueueSortCommand<T, TT>> SortCommands { get; private set; }

        protected DepthSortManager(TT container)
            : base(container)
        {
            SortCommands = new Queue<ManagedQueueSortCommand<T, TT>>();
        }

        protected override void PostProcessQueues()
        {
            while (SortCommands.Count > 0)
                TryMove(SortCommands.Dequeue());
        }

        public void MoveToTop(T item)
        {
            if (!Items.Contains(item))
                return;

            var command = new ManagedQueueSortCommand<T, TT>(item, ManagedQueueSortAction.MoveToTop);

            if (!SortCommands.Contains(command))
                SortCommands.Enqueue(command);
        }

        public void MoveToBottom(T item)
        {
            if (!Items.Contains(item))
                return;

            var command = new ManagedQueueSortCommand<T, TT>(item, ManagedQueueSortAction.MoveToBottom);

            if (!SortCommands.Contains(command))
                SortCommands.Enqueue(command);
        }

        public void MoveAbove(T item, T other)
        {
            if (item.Equals(other) || !Items.Contains(item) || !Items.Contains(other))
                return;

            var command = new ManagedQueueSortCommand<T, TT>(item, other, ManagedQueueSortAction.MoveAbove);

            if (!SortCommands.Contains(command))
                SortCommands.Enqueue(command);
        }

        public void MoveBelow(T item, T other)
        {
            if (item.Equals(other) || !Items.Contains(item) || !Items.Contains(other))
                return;

            var command = new ManagedQueueSortCommand<T, TT>(item, other, ManagedQueueSortAction.MoveBelow);

            if (!SortCommands.Contains(command))
                SortCommands.Enqueue(command);
        }

        protected void TryMove(ManagedQueueSortCommand<T, TT> command)
        {
            switch (command.Action)
            {
                case ManagedQueueSortAction.MoveToTop:
                    if (Items.Contains(command.Item))
                    {
                        Items.Remove(command.Item);
                        Items.Add(command.Item);
                    }
                    break;
                case ManagedQueueSortAction.MoveToBottom:
                    if (Items.Contains(command.Item))
                    {
                        Items.Remove(command.Item);
                        Items.Insert(0, command.Item);
                    }
                    break;
                case ManagedQueueSortAction.MoveAbove:
                    if (Items.Contains(command.Item) && Items.Contains(command.Target))
                    {
                        Items.Remove(command.Item);
                        Items.Insert(Items.IndexOf(command.Target) + 1, command.Item);
                    }
                    break;
                case ManagedQueueSortAction.MoveBelow:
                    if (Items.Contains(command.Item) && Items.Contains(command.Target))
                    {
                        Items.Remove(command.Item);
                        Items.Insert(Items.IndexOf(command.Target), command.Item);
                    }
                    break;
            }
        }
    }

    public struct ManagedQueueSortCommand<T, TT> : IManagedQueueCommand<T, ManagedQueueSortAction>
        where T : Managable<TT> where TT : class
    {
        public T Item { get; }

        public T Target { get; }

        public ManagedQueueSortAction Action { get; }

        internal ManagedQueueSortCommand(T item, ManagedQueueSortAction action)
            : this(item, null, action) { }

        internal ManagedQueueSortCommand(T item, T target, ManagedQueueSortAction action)
            : this()
        {
            Item = item;
            Target = target;
            Action = action;
        }
    }

    public enum ManagedQueueSortAction
    {
        MoveToTop,
        MoveToBottom,
        MoveAbove,
        MoveBelow
    }
}
