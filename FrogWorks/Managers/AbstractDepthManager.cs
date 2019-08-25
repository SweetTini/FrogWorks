using System.Collections.Generic;

namespace FrogWorks
{
    public abstract class AbstractDepthManager<TItem, TContainer> : AbstractManager<TItem, TContainer>
        where TItem : AbstractManageable<TContainer>
        where TContainer : class
    {
        protected Queue<MoveItemCommand> ToMove { get; private set; }

        protected AbstractDepthManager(TContainer container)
            : base(container)
        {
            ToMove = new Queue<MoveItemCommand>();
        }

        protected override void PostProcessQueues()
        {
            while (ToMove.Count > 0)
                TryMove(ToMove.Dequeue());
        }

        public void MoveToTop(TItem item)
        {
            if (!Items.Contains(item))
                return;

            var command = new MoveItemCommand(item, MoveOperation.ToTop);

            if (IsBusy && !ToMove.Contains(command))
                ToMove.Enqueue(command);
            else if (!IsBusy)
                TryMove(command);
        }

        public void MoveToBottom(TItem item)
        {
            if (!Items.Contains(item))
                return;

            var command = new MoveItemCommand(item, MoveOperation.ToBottom);

            if (IsBusy && !ToMove.Contains(command))
                ToMove.Enqueue(command);
            else if (!IsBusy)
                TryMove(command);
        }

        public void MoveAhead(TItem item, TItem other)
        {
            if (item.Equals(other) || !Items.Contains(item) || !Items.Contains(other))
                return;

            var command = new MoveItemCommand(item, other, MoveOperation.Ahead);

            if (IsBusy && !ToMove.Contains(command))
                ToMove.Enqueue(command);
            else if (!IsBusy)
                TryMove(command);
        }

        public void MoveBehind(TItem item, TItem other)
        {
            if (item.Equals(other) || !Items.Contains(item) || !Items.Contains(other))
                return;

            var command = new MoveItemCommand(item, other, MoveOperation.Behind);

            if (IsBusy && !ToMove.Contains(command))
                ToMove.Enqueue(command);
            else if (!IsBusy)
                TryMove(command);
        }

        protected void TryMove(MoveItemCommand command)
        {
            switch (command.Operation)
            {
                case MoveOperation.ToTop:
                    if (Items.Contains(command.Item))
                    {
                        Items.Remove(command.Item);
                        Items.Add(command.Item);
                    }
                    break;
                case MoveOperation.ToBottom:
                    if (Items.Contains(command.Item))
                    {
                        Items.Remove(command.Item);
                        Items.Insert(0, command.Item);
                    }
                    break;
                case MoveOperation.Ahead:
                    if (Items.Contains(command.Item) && Items.Contains(command.Target))
                    {
                        Items.Remove(command.Item);
                        Items.Insert(Items.IndexOf(command.Target) + 1, command.Item);
                    }
                    break;
                case MoveOperation.Behind:
                    if (Items.Contains(command.Item) && Items.Contains(command.Target))
                    {
                        Items.Remove(command.Item);
                        Items.Insert(Items.IndexOf(command.Target), command.Item);
                    }
                    break;
            }
        }

        protected class MoveItemCommand
        {
            public TItem Item { get; set; }

            public TItem Target { get; set; }

            public MoveOperation Operation { get; set; }

            public MoveItemCommand(TItem item, MoveOperation operation)
                : this(item, null, operation) { }

            public MoveItemCommand(TItem item, TItem target, MoveOperation operation)
            {
                Item = item;
                Target = target;
                Operation = operation;
            }
        }

        protected enum MoveOperation
        {
            ToTop,
            ToBottom,
            Ahead,
            Behind
        }
    }
}
