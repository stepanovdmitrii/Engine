using System;
using System.Threading;
using System.Collections.Generic;

namespace Engine.Core.Concurrent
{
    public sealed class KeyMonitor<TKey>
    {
        private readonly object _dictionaryLock = new object();
        private readonly Dictionary<TKey, KeyLock> _locks;

        public KeyMonitor(IEqualityComparer<TKey> equalityComparer)
        {
            Verify.IsNotNull(equalityComparer, nameof(equalityComparer));
            _locks = new Dictionary<TKey, KeyLock>(equalityComparer);
        }

        public KeyMonitor(): this(EqualityComparer<TKey>.Default) { }

        public IDisposable Lock(TKey key)
        {
            KeyLock keyLock = null;
            lock (_dictionaryLock)
            {
                if(!_locks.TryGetValue(key, out keyLock))
                {
                    keyLock = new KeyLock();
                    _locks[key] = keyLock;
                }
                keyLock.Increment();
            }
            Monitor.Enter(keyLock.Token);
            return new DisposableAction(() => Unlock(key));
        }

        private void Unlock(TKey key)
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

            public void Increment()
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
