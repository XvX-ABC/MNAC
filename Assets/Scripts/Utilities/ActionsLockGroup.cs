using System;
using System.Linq;
namespace Utilities
{
    public class ActionsLockGroup<T> : IActionsLock<T> where T : Enum
    {
        protected IActionsLock<T>[] _subLocks;
        public ActionsLockGroup(params IActionsLock<T>[] subLocks)
        {
            _subLocks = subLocks;
        }

        public bool AnyLocked()
        {
            return _subLocks.Any(l => l.AnyLocked());
        }

        public bool IsLocked(T e)
        {
            return _subLocks.Any(l => l.IsLocked(e));
        }

        public void Lock(params T[] es)
        {
            foreach (var l in _subLocks)
                l.Lock(es);
        }

        public void LockAll()
        {
            foreach (var l in _subLocks)
                l.LockAll();
        }

        public void Unlock(params T[] es)
        {
            foreach (var l in _subLocks)
                l.Unlock(es);
        }

        public void UnlockAll()
        {
            foreach (var l in _subLocks)
                l.UnlockAll();
        }
    }
}