using System.Threading;

namespace Engine.Core.Concurrent
{
    public sealed class ReadWriteLock
    {
        private readonly object _lock;
        private int _readers;
        private bool _writeLockRequested;

        public ReadWriteLock()
        {
            _lock = new object();
            _readers = 0;
            _writeLockRequested = false;
        }

        public void EnterWriteLock()
        {
            lock (_lock)
            {
                while (_writeLockRequested)
                {
                    Monitor.Wait(_lock);
                }
                _writeLockRequested = true;
                while (_readers > 0)
                {
                    Monitor.Wait(_lock);
                }
            }
        }

        public void EnterReadLock()
        {
            lock (_lock)
            {
                while (_writeLockRequested)
                {
                    Monitor.Wait(_lock);
                }
                _readers++;
            }
        }

        public void ExitWriteLock()
        {
            lock (_lock)
            {
                _writeLockRequested = false;
                Monitor.PulseAll(_lock);
            }
        }

        public void ExitReadLock()
        {
            lock (_lock)
            {
                _readers--;
                if(_readers == 0)
                {
                    Monitor.PulseAll(_lock);
                }
            }
        }
    }
}
