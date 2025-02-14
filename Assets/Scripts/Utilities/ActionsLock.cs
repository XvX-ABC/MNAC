using System;
using UnityEngine;
namespace Utilities
{
    public class ActionsLock<T> : IActionsLock<T> where T : Enum
    {
        byte _lockArray;
        Type _enumType;
        public ActionsLock(bool defaultValue = false)
        {
            _enumType = typeof(T);
            var length = Enum.GetNames(_enumType).Length;
            if (length >= 8)
                throw new Exception();
            _lockArray = defaultValue ? byte.MaxValue : (byte)0;
        }
        public bool IsLocked(T e)
        {
            var v = Convert.ToByte(e);
            return (_lockArray & v) == v;
        }
        public bool AnyLocked()
        {
            return _lockArray > 0;
        }
        public void Lock(params T[] es)
        {
            foreach (var e in es)
                _lockArray |= Convert.ToByte(e);
        }
        public void Unlock(params T[] es)
        {
            foreach (var e in es)
            {
                var v = Convert.ToByte(e);
                if ((_lockArray & v) == v)
                    _lockArray ^= v;
            }
        }

        public void LockAll()
        {
            _lockArray = byte.MaxValue;
        }
        public void UnlockAll()
        {
            _lockArray = 0;
        }
    }
}