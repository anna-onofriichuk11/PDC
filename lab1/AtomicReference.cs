using System;
using System.Threading;

namespace lab1
{
  public class AtomicReference<T> where T : class
        {
            private object _lock = new object();
            private T _value;
            public AtomicReference(T value) => _value = value;
            public void LockedUpdate(Func<T,T> updateFunc) 
            {
                lock(_lock)
                {
                    _value = updateFunc(_value);
                }
            }

            public void Set(T value) => _value = value;
            
            public T Value
            {
                get
                {
                    object obj = _value;
                    return (T)Thread.VolatileRead(ref obj);
                }
            }
            
            public void LockedAccess(Action<T> action)
            {
                lock(_lock)
                {
                    action(_value);
                }
            }
            public TResult LockedAccess<TResult>(Func<T, TResult> accessAndReturnResult)
            {
                lock(_lock)
                {
                    return accessAndReturnResult(_value);
                }
            }
            public TResult LockedAccessAndUpdate<TResult>(Func<T, (T, TResult)> accessAndUpdate)
            {
                lock(_lock)
                {
                    TResult result;
                    (_value, result) = accessAndUpdate(_value);
                    return result;
                }
            }
            
            public bool CompareAndExchange(T newValue, T oldValue)
            {
                return ReferenceEquals(Interlocked.CompareExchange(ref _value, newValue, oldValue), oldValue);
            }
        }
    }
