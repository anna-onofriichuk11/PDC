using System;

namespace lab1.Task3
{
    public interface INonBlockingQueue<T>
    {
        void Add(T elem);
        T Remove();
    }
}