using System;
using System.Collections.Generic;
using System.Threading;

namespace lab1.Task1
{
    public class Mutex : IMutex
    { 
        private int _locked = 0;
        
        private Thread _currentThread; 
        
        private IList<Thread> _waitingThreadList = new List<Thread>();
        
        public void Wait()
        {
            _waitingThreadList.Add(Thread.CurrentThread);
            
            UnlockThread();

            while (_locked == 0)
            {
                Thread.Yield();
            }
            
            LockThread();
            Interlocked.Exchange(ref _locked, 1);
        }

        public void Notify()
        {
            _currentThread = Thread.CurrentThread;
            
            if (_waitingThreadList.Count < 0)
            {
                throw new NullReferenceException();
            }
            
            _waitingThreadList.RemoveAt(_waitingThreadList.Count);
            Interlocked.Exchange(ref _locked, 0);
            
        }

        public void NotifyAll()
        {
            _waitingThreadList.Clear();
        }

        public void LockThread()
        {
            Interlocked.Exchange(ref _locked , 1);
            while (Interlocked.CompareExchange(ref _currentThread, Thread.CurrentThread, null) == null)
            {
                Thread.Yield();
            }
        }

        public void UnlockThread()
        {
            Interlocked.Exchange(ref _locked, 0);
            Interlocked.Exchange(ref _currentThread, null);
        }
    }
}