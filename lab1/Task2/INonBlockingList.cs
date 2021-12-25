using System.Collections.Generic;

namespace lab1.Task2
{
    public interface INonBlockingList<T>
    {
        void Add(T elem);
        bool Remove(T elem);
        bool Contains(T elem);
    }
}