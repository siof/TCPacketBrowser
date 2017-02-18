using System;
using System.Threading;

namespace WpfCommons.Locks
{
    public class WriteLock : IDisposable
    {
        private ReaderWriterLockSlim _lock;

        public WriteLock(ReaderWriterLockSlim lockItem)
        {
            _lock = lockItem;
            _lock.EnterWriteLock();
        }

        public void Dispose()
        {
            _lock.ExitWriteLock();
            _lock = null;
        }
    }
}
