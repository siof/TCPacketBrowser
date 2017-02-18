using System;
using System.Threading;

namespace WpfCommons.Locks
{
    public class ReadLock : IDisposable
    {
        private ReaderWriterLockSlim _lock;

        public ReadLock(ReaderWriterLockSlim lockItem)
        {
            _lock = lockItem;
            _lock.EnterUpgradeableReadLock();
        }

        public void Dispose()
        {
            _lock.ExitUpgradeableReadLock();
            _lock = null;
        }
    }
}
