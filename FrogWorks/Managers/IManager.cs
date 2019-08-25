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

        void Add(IEnumerable<T> items);

        void Remove(T item);

        void Remove(IEnumerable<T> items);

        T[] ToArray();
    }
}
