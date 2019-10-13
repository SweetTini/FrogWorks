using System.Collections;
using System.Collections.Generic;

namespace FrogWorks
{
    public interface IManager<T> : IEnumerable<T>, IEnumerable
        where T : class
    {
        T this[int index] { get; }

        int Count { get; }

        void Add(T item);

        void Add(params T[] items);

        void Add(IEnumerable<T> items);

        void Remove(T item);

        void Remove(params T[] items);

        void Remove(IEnumerable<T> items);

        T[] ToArray();
    }

    public interface IManagedQueueCommand<T>
        where T : class
    {
        T Item { get; }
    }

    public interface IManagedQueueCommand<T, E> : IManagedQueueCommand<T>
        where T : class where E : struct
    {
        E Action { get; }
    }
}
