using System;
using Engine.Core.Concurrent;

namespace Engine.Core.Extensions
{
    public static class ReadWriteLockExtension
    {
        public static IDisposable GetReadLock(this ReadWriteLock readWriteLock)
        {
            readWriteLock.EnterReadLock();
            return new DisposableAction(() => readWriteLock.ExitReadLock());
        }

        public static IDisposable GetWriteLock(this ReadWriteLock readWriteLock)
        {
            readWriteLock.EnterWriteLock();
            return new DisposableAction(() => readWriteLock.ExitWriteLock());
        }
    }
}
