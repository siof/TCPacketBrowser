using System;
using System.Threading;

namespace WpfCommons.Locks
{
    public class ReadOnlyLock : IDisposable
    {
        private ReaderWriterLockSlim _lock;

        public ReadOnlyLock(ReaderWriterLockSlim lockItem)
        {
            _lock = lockItem;
            _lock.EnterReadLock();
        }

        public void Dispose()
        {
            _lock.ExitReadLock();
            _lock = null;
        }
    }
}
