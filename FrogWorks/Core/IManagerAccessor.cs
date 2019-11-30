using System.Collections;
using System.Collections.Generic;

namespace FrogWorks
{
    public interface IManagerAccessor<T> : IEnumerable<T>, IEnumerable
        where T : class
    {
        void Add(T item);

        void Add(params T[] items);

        void Add(IEnumerable<T> items);

        void Remove(T item);

        void Remove(params T[] items);

        void Remove(IEnumerable<T> items);
    }
}
