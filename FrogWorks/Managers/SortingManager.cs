using System.Collections.Generic;

namespace FrogWorks
{
    public abstract class SortingManager<C, P> : Manager<C, P>
        where C : Manageable<P>
        where P : class
    {
        protected Queue<SortingManagerCommand<C, P>> SortCommands { get; private set; }

        protected SortingManager(P parent)
            : base(parent)
        {
            SortCommands = new Queue<SortingManagerCommand<C, P>>();
        }

        protected override void PostProcessQueues()
        {
            while (SortCommands.Count > 0)
                TrySort(SortCommands.Dequeue());
        }

        protected void TrySort(SortingManagerCommand<C, P> command)
        {
            switch (command.Type)
            {
                case SortingManagerCommandType.MoveToTop:
                    if (Children.Contains(command.Child))
                    {
                        Children.Remove(command.Child);
                        Children.Add(command.Child);
                    }
                    break;
                case SortingManagerCommandType.MoveToBottom:
                    if (Children.Contains(command.Child))
                    {
                        Children.Remove(command.Child);
                        Children.Insert(0, command.Child);
                    }
                    break;
                case SortingManagerCommandType.MoveAbove:
                    if (Children.Contains(command.Child)
                        && Children.Contains(command.Target))
                    {
                        Children.Remove(command.Child);

                        var targetIndex = Children.IndexOf(command.Target) + 1;
                        Children.Insert(targetIndex, command.Child);
                    }
                    break;
                case SortingManagerCommandType.MoveBelow:
                    if (Children.Contains(command.Child)
                        && Children.Contains(command.Target))
                    {
                        Children.Remove(command.Child);

                        var targetIndex = Children.IndexOf(command.Target);
                        Children.Insert(targetIndex, command.Child);
                    }
                    break;
            }
        }

        public void MoveToTop(C child)
        {
            if (Contains(child))
            {
                var command = new SortingManagerCommand<C, P>(
                    child, SortingManagerCommandType.MoveToTop);

                if (!SortCommands.Contains(command))
                    SortCommands.Enqueue(command);
            }
        }

        public void MoveToBottom(C child)
        {
            if (Contains(child))
            {
                var command = new SortingManagerCommand<C, P>(
                    child, SortingManagerCommandType.MoveToBottom);

                if (!SortCommands.Contains(command))
                    SortCommands.Enqueue(command);
            }
        }

        public void MoveAbove(C child, C target)
        {
            if (child != target && Contains(child) && Contains(target))
            {
                var command = new SortingManagerCommand<C, P>(
                    child, target, SortingManagerCommandType.MoveAbove);

                if (!SortCommands.Contains(command))
                    SortCommands.Enqueue(command);
            }
        }

        public void MoveBelow(C child, C target)
        {
            if (child != target && Contains(child) && Contains(target))
            {
                var command = new SortingManagerCommand<C, P>(
                    child, target, SortingManagerCommandType.MoveBelow);

                if (!SortCommands.Contains(command))
                    SortCommands.Enqueue(command);
            }
        }
    }

    public struct SortingManagerCommand<C, P>
        where C : Manageable<P>
        where P : class
    {
        public C Child { get; }

        public C Target { get; }

        public SortingManagerCommandType Type { get; }

        internal SortingManagerCommand(C child, SortingManagerCommandType type)
            : this(child, null, type)
        {
        }

        internal SortingManagerCommand(C child, C target, SortingManagerCommandType type)
            : this()
        {
            Child = child;
            Target = target;
            Type = type;
        }
    }

    public enum SortingManagerCommandType
    {
        MoveToTop,
        MoveToBottom,
        MoveAbove,
        MoveBelow
    }
}
