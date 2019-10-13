namespace FrogWorks
{
    public class ManagedQueueCommand<T, TT> 
        where T : Managable<TT> 
        where TT : class
    {
        public T Item { get; private set; }

        public ManagedQueueAction Action { get; private set; }

        internal ManagedQueueCommand(T item, ManagedQueueAction action)
        {
            Item = item;
            Action = action;
        }
    }

    public enum ManagedQueueAction
    {
        None,
        Add,
        Remove,
        MoveToTop,
        MoveToBottom,
        MoveAbove,
        MoveBelow,
        Switch
    }
}
