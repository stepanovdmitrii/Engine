using System;
using System.Collections.Generic;
using System.Threading;

namespace Engine.Core.Concurrent
{
    public sealed class ReadWriteKeyMonitor<TKey> : IDisposable
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
            ILockContext context = null;
            bool lockTaken = false;
            try
            {
                context = _locks.Acquire(key);
                if (exclusive)
                {
                    context.EnterWriteLock();
                }
                else
                {
                    context.EnterReadLock();
                }
                lockTaken = true;
                context = null;
            }
            finally
            {
                if(context != null)
                {
                    if (lockTaken)
                    {
                        _locks.ReleaseAndUnlock(key, exclusive);
                    }
                    else
                    {
                        _locks.Release(key);
                    }
                }
            }
            return new DisposableAction(() => _locks.ReleaseAndUnlock(key, exclusive));
        }

        public void Dispose()
        {
            if (_locks != null) _locks.Dispose();
        }

        private interface ILockContext
        {
            void EnterReadLock();
            void EnterWriteLock();
        }

        private sealed class LockSet : IDisposable
        {
            private readonly object _lock = new object();
            private readonly Dictionary<TKey, LockContext> _locks;
            private bool _disposed = false;

            public LockSet(IEqualityComparer<TKey> equalityComparer)
            {
                _locks = new Dictionary<TKey, LockContext>(equalityComparer);
            }

            public ILockContext Acquire(TKey key)
            {
                Verify.IsNotDisposed(_disposed, this);
                lock (_lock)
                {
                    LockContext context = null;
                    try
                    {
                        if(!_locks.TryGetValue(key, out context))
                        {
                            LockContext contextToAdd = null;
                            try
                            {
                                contextToAdd = new LockContext();
                                _locks[key] = contextToAdd;
                                context = contextToAdd;
                                contextToAdd = null;
                            }
                            finally
                            {
                                if(contextToAdd != null)
                                {
                                    contextToAdd.Dispose();
                                    _locks.Remove(key);
                                }
                            }
                        }
                        context.IncrementUsages();
                        var temp = context;
                        context = null;
                        return temp;
                    }
                    finally
                    {
                        if(context != null)
                        {
                            context.Dispose();
                            _locks.Remove(key);
                        }
                    }
                }
            }

            public void ReleaseAndUnlock(TKey key, bool exclusive)
            {
                Verify.IsNotDisposed(_disposed, this);
                LockContext context = null;
                lock (_lock)
                {
                    try
                    {
                        context = _locks[key];
                        if (exclusive)
                        {
                            context.ExitWriteLock();
                        }
                        else
                        {
                            context.ExitReadLock();
                        }
                        if(context.DecrementUsages() != 0)
                        {
                            context = null;
                        }
                    }
                    finally
                    {
                        if(context != null)
                        {
                            context.Dispose();
                            _locks.Remove(key);
                        }
                    }
                }
            }

            public void Release(TKey key)
            {
                Verify.IsNotDisposed(_disposed, this);
                LockContext context = null;
                lock (_lock)
                {
                    try
                    {
                        context = _locks[key];
                        if (context.DecrementUsages() != 0)
                        {
                            context = null;
                        }
                    }
                    finally
                    {
                        if (context != null)
                        {
                            context.Dispose();
                            _locks.Remove(key);
                        }
                    }
                }
            }

            public void Dispose()
            {
                if (_disposed)
                    return;

                foreach(var pair in _locks)
                {
                    pair.Value.Dispose();
                }
                _locks.Clear();
                _disposed = true;
            }

            private sealed class LockContext : ILockContext, IDisposable
            {
                private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
                private uint _usages = 0;
                private bool _disposed = false;

                public void IncrementUsages()
                {
                    Verify.IsNotDisposed(_disposed, this);
                    _usages++;
                }

                public uint DecrementUsages()
                {
                    Verify.IsNotDisposed(_disposed, this);
                    _usages--;
                    return _usages;
                }

                public void EnterReadLock()
                {
                    Verify.IsNotDisposed(_disposed, this);
                    _lock.EnterReadLock();
                }

                public void ExitReadLock()
                {
                    Verify.IsNotDisposed(_disposed, this);
                    _lock.ExitReadLock();
                }

                public void EnterWriteLock()
                {
                    Verify.IsNotDisposed(_disposed, this);
                    _lock.EnterWriteLock();
                }

                public void ExitWriteLock()
                {
                    Verify.IsNotDisposed(_disposed, this);
                    _lock.ExitWriteLock();
                }

                public void Dispose()
                {
                    if (_disposed)
                        return;

                    if (_lock != null)
                    {
                        _lock.Dispose();
                    }
                    _disposed = true;
                }
            }
        }
    }
}
