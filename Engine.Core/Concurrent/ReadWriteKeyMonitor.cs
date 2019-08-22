using System;
using System.Collections.Generic;

namespace Engine.Core.Concurrent
{
    public sealed class ReadWriteKeyMonitor<TKey>
    {
        private readonly LockSet _locks;

        public ReadWriteKeyMonitor(IEqualityComparer<TKey> equalityComparer)
        {
            _locks = new LockSet(equalityComparer);
        }

        public ReadWriteKeyMonitor(): this(EqualityComparer<TKey>.Default) { }

        public IDisposable LockForRead(TKey key)
        {
            return GetLock(key, false);
        }

        public IDisposable LockForWrite(TKey key)
        {
            return GetLock(key, true);
        }

        private IDisposable GetLock(TKey key, bool exclusive)
        {
            LockContext context = _locks.Acquire(key);
            if (exclusive)
            {
                context.EnterWriteLock();
            }
            else
            {
                context.EnterReadLock();
            }
            return new DisposableAction(() => _locks.ReleaseAndUnlock(key, exclusive));           
        }

        private sealed class LockContext
        {
            private readonly ReadWriteLock _lock = new ReadWriteLock();
            private uint _usages = 0;

            public void IncrementUsages()
            {
                _usages++;
            }

            public uint DecrementUsages()
            {
                _usages--;
                return _usages;
            }

            public void EnterReadLock()
            {
                _lock.EnterReadLock();
            }

            public void ExitReadLock()
            {
                _lock.ExitReadLock();
            }

            public void EnterWriteLock()
            {
                _lock.EnterWriteLock();
            }

            public void ExitWriteLock()
            {
                _lock.ExitWriteLock();
            }
        }

        private sealed class LockSet
        {
            private readonly object _lock = new object();
            private readonly Dictionary<TKey, LockContext> _locks;

            public LockSet(IEqualityComparer<TKey> equalityComparer)
            {
                _locks = new Dictionary<TKey, LockContext>(equalityComparer);
            }

            public LockContext Acquire(TKey key)
            {
                lock (_lock)
                {
                    if (!_locks.TryGetValue(key, out LockContext context))
                    {
                        context = new LockContext();
                        _locks[key] = context;
                    }
                    context.IncrementUsages();
                    return context;
                }
            }

            public void ReleaseAndUnlock(TKey key, bool exclusive)
            {
                lock (_lock)
                {
                    LockContext context = _locks[key];
                    if (exclusive)
                    {
                        context.ExitWriteLock();
                    }
                    else
                    {
                        context.ExitReadLock();
                    }
                    if (context.DecrementUsages() == 0)
                    {
                        _locks.Remove(key);
                    }
                }
            }
        }
    }
}
