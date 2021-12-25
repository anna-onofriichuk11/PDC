using System.Collections.Generic;
using System.Linq;
using lab1.Task3;
using NUnit.Framework;

namespace lab1Tests
{
    public class LockFreeQueueTests
    {
        private MsQueue<int> _queue;
        private SynchronizedCollection<int> _addedValues;
        private SynchronizedCollection<int> _removedVales;
        private Setup _setup;
        
        [SetUp]
        public void SetUp()
        {
            _queue = new MsQueue<int>();
            _addedValues = new SynchronizedCollection<int>();
            _removedVales = new SynchronizedCollection<int>();
            _setup = new Setup();
        }

        [Test]
        public void LockFreeQueueTestPerformance()
        {
            _setup.RunActions(AddToCollection, 10);
            _setup.RunActions(RemoveFromCollection, 10);
            var sortedAddedValues = _addedValues.OrderBy(x => x);
            var sortedRemovedValues = _removedVales.OrderBy(x => x);
            
            CollectionAssert.AreEqual(sortedAddedValues, sortedRemovedValues);
        }

        private void AddToCollection(object? obj)
        {
            _queue.Add((int) obj);
            _addedValues.Add((int) obj);
        }

        private void RemoveFromCollection(object? obj)
        {
            var result = _queue.Remove();
            if (result != null)
            {
                _removedVales.Add(result);
            }
        }
    }
}