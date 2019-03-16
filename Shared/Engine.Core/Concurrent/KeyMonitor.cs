using System;
using System.Threading;
using System.Collections.Generic;

namespace Engine.Core.Concurrent
{
    public sealed class KeyMonitor<T>
    {
        private readonly object _dictionaryLock = new object();
        private readonly Dictionary<T, KeyLock> _locks;

        public KeyMonitor(IEqualityComparer<T> equalityComparer)
        {
            Verify.IsNotNull(equalityComparer, nameof(equalityComparer));
            _locks = new Dictionary<T, KeyLock>(equalityComparer);
        }

        public KeyMonitor(): this(EqualityComparer<T>.Default) { }

        public IDisposable Lock(T key)
        {
            KeyLock keyLock = null;
            lock (_dictionaryLock)
            {
                if(!_locks.TryGetValue(key, out keyLock))
                {
                    keyLock = new KeyLock();
                    _locks[key] = keyLock;
                }
                keyLock.AddReference();
            }
            Monitor.Enter(keyLock.Token);
            return new DisposableAction(() => Unlock(key));
        }

        private void Unlock(T key)
        {
            lock (_dictionaryLock)
            {
                Verify.Assert(_locks.TryGetValue(key, out KeyLock keyLock));
                if(keyLock.Release() == 0)
                {
                    _locks.Remove(key);
                }
                Monitor.Exit(keyLock.Token);
            }
        }

        private sealed class KeyLock
        {
            private int _refs = 0;
            public readonly object Token = new object();

            public void  AddReference()
            {
                ++_refs;
            }

            public int Release()
            {
                --_refs;
                return _refs;
            }
        }
    }
}
