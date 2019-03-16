using System.Collections.Generic;
using System.Threading;

namespace Engine.Core.Collections
{
    public sealed class FixedSizeQueue<T>
    {
        private readonly object _lock = new object();
        private readonly Queue<T> _queue;
        private readonly int _capacity;
        private bool _closed;

        public FixedSizeQueue(int capacity)
        {
            Verify.InRangeInclude(capacity, 1, int.MaxValue, nameof(capacity));
            _capacity = capacity;
            _closed = false;
            _queue = new Queue<T>(_capacity);
        }

        public void Enqueue(T obj)
        {
            lock (_lock)
            {
                Verify.Assert(!_closed);
                while (_queue.Count == _capacity)
                {
                    Monitor.Wait(_lock);
                }
                _queue.Enqueue(obj);
                if (_queue.Count == 1)
                {
                    Monitor.PulseAll(_lock);
                }
            }
        }

        public bool Dequeue(out T value)
        {
            lock (_lock)
            {
                while (_queue.Count == 0)
                {
                    if (_closed)
                    {
                        value = default(T);
                        return false;
                    }
                    Monitor.Wait(_lock);
                }
                value = _queue.Dequeue();
                if (_queue.Count == _capacity - 1)
                {
                    Monitor.PulseAll(_lock);
                }
                return true;
            }
        }

        public void Close()
        {
            if (_closed)
                return;

            lock (_lock)
            {
                if (_closed)
                    return;

                _closed = true;
                Monitor.PulseAll(_lock);
            }
        }
    }
}
