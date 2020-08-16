using System;
using System.Collections.Generic;
using System.Threading;

namespace Engine.Core.Concurrent
{
    public interface IReadWriteKeyMonitor<TKey>
    {
        IDisposable AcquireReadLock(TKey key);
        IDisposable AcquireWriteLock(TKey key);
    }

    public sealed class ReadWriteKeyMonitor<TKey>: IReadWriteKeyMonitor<TKey>, IDisposable
    {
        private readonly object _global;
        private readonly Dictionary<TKey, LockContext> _locks;

        public ReadWriteKeyMonitor(IEqualityComparer<TKey> equalityComparer)
        {
            _global = new object();
            _locks = new Dictionary<TKey, LockContext>(equalityComparer);
        }

        public ReadWriteKeyMonitor(): this(EqualityComparer<TKey>.Default) { }

        public IDisposable AcquireReadLock(TKey key)
        {
            return GetLock(key, LockType.Read);
        }

        public IDisposable AcquireWriteLock(TKey key)
        {
            return GetLock(key, LockType.Write);
        }

        private IDisposable GetLock(TKey key, LockType type)
        {
            ILockContext context = GetOrAdd(key);

            try
            {
                context.EnterLock(type);
            }
            catch
            {
                ReleaseFailed(key);
                throw;
            }
            return new DisposableAction(() => Release(key, type));
        }

        private void Release(TKey key, LockType type)
        {
            lock (_global)
            {
                Verify.Assert(_locks.TryGetValue(key, out LockContext context));
                if (context.DecrementUsages() == 0)
                {
                    using (context)
                    {
                        _locks.Remove(key);
                        context.ExitLock(type);
                    }
                }
                else
                {
                    context.ExitLock(type);
                }
            }
        }

        private void ReleaseFailed(TKey key)
        {
            lock (_global)
            {
                Verify.Assert(_locks.TryGetValue(key, out LockContext context));
                if (context.DecrementUsages() == 0)
                {
                    using (context)
                    {
                        _locks.Remove(key);
                    }
                }
            }
        }

        private ILockContext GetOrAdd(TKey key)
        {
            lock (_global)
            {
                if (false == _locks.TryGetValue(key, out LockContext context))
                {
                    LockContext add = null;
                    try
                    {
                        add = new LockContext();
                        _locks[key] = add;
                        context = add;
                        add = null;
                    }
                    finally
                    {
                        if (add != null) add.Dispose();
                    }
                }
                context.IncrementUsages();
                return context;
            }
        }

        public void Dispose()
        {
            foreach(var pair in _locks)
            {
                pair.Value.Dispose();
            }
            _locks.Clear();
        }

        private enum LockType
        {
            Read = 0,
            Write = 1,
            //Upgradeable = 2
        }

        private interface ILockContext
        {
            void IncrementUsages();
            void EnterLock(LockType type);
        }

        private sealed class LockContext: ILockContext, IDisposable
        {
            private readonly ReaderWriterLockSlim _lock;
            private uint _usages;

            public LockContext()
            {
                _lock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
                _usages = 0;
            }

            public void IncrementUsages()
            {
                Verify.Assert(_usages < uint.MaxValue);
                _usages++;
            }

            public uint DecrementUsages()
            {
                Verify.Assert(_usages > uint.MinValue);
                _usages--;
                return _usages;
            }

            public void EnterLock(LockType type)
            {
                switch (type)
                {
                    case LockType.Read:
                        _lock.EnterReadLock();
                        break;
                    case LockType.Write:
                        _lock.EnterWriteLock();
                        break;
                    default:
                        Verify.Assert(false);
                        return;
                }
            }

            public void ExitLock(LockType type)
            {
                switch (type)
                {
                    case LockType.Read:
                        _lock.ExitReadLock();
                        break;
                    case LockType.Write:
                        _lock.ExitWriteLock();
                        break;
                    default:
                        Verify.Assert(false);
                        return;
                }
            }

            public void Dispose()
            {
                if (_lock != null) _lock.Dispose();
            }
        }
    }
}
