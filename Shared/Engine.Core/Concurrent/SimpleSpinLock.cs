using System;
using System.Threading;

namespace Engine.Core.Concurrent
{
    public sealed class SimpleSpinLock
    {
        private volatile int _locked = 0;

        public IDisposable Lock()
        {
            while (Interlocked.CompareExchange(ref _locked, 1, 0) != 0)
                continue;

            return new DisposableAction(Unlock);
        }

        private void Unlock()
        {
            _locked = 0;
        }
    }
}
