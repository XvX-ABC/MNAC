using System;

namespace Tests.Utilities
{
    public interface IActionsLock<T> where T : Enum
    {
        bool AnyLocked();
        bool IsLocked(T e);
        void Lock(params T[] es);
        void LockAll();
        void Unlock(params T[] es);
        void UnlockAll();
    }
}