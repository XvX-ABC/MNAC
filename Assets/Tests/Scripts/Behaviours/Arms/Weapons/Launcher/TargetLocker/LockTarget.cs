using Tests.Interaction;
using Tests.UI;
using UnityEngine;
using UnityEngine.Pool;
using ULockType = Tests.UI.BoxIndicator.LockType;
namespace Tests.Behaviours.Arms.Weapons.Launcher
{
    internal class LockTarget : ILockTarget
    {
        static ObjectPool<LockTarget> s_pool;
        static GameObject _currentObj;
        static LockType _currentType;
        static LockTarget()
        {
            s_pool = new(CreateInstance, WhenGetInstance, WhenReleaseInstance, DestroyInstance, false, 5, 1000);
        }
        public static LockTarget GetInstance(GameObject obj, LockType type)
        {
            _currentObj = obj;
            _currentType = type;
            var result = s_pool.Get();
            _currentObj = null;
            _currentType = default;
            return result;
        }
        public static void ReleaseInstance(LockTarget target)
        {
            s_pool.Release(target);
        }
        static LockTarget CreateInstance()
        {
            return new();
        }
        static void WhenGetInstance(LockTarget target)
        {
            target.Obj = _currentObj;
            target.LockType = _currentType;
        }
        static void WhenReleaseInstance(LockTarget target)
        {
            target.Obj = null;
            target.LockType = LockType.None;
        }
        static void DestroyInstance(LockTarget target)
        {

        }
        public static explicit operator GameObject(LockTarget target)
        {
            return target._obj;
        }
        GameObject _obj;
        LockType _lockType;
        internal IndicatedTarget indicatedTarget;
        internal BoxIndicator indicator => (BoxIndicator)indicatedTarget?.Indicator;
        public GameObject Obj { get => _obj; set => _obj = value; }
        public LockType LockType
        {
            get => _lockType;
            set
            {
                if (indicator != null)
                {
                    indicator.Type = value switch
                    {
                        LockType.None => ULockType.Unlock,
                        LockType.CantLock => ULockType.Unlock,
                        LockType.Lock_Unconfirm => ULockType.Lock_Unconfirm,
                        LockType.Lock_Confirmed => ULockType.Lock_Confirmed,
                    };
                }
                _lockType = value;
            }
        }

        public Vector3 Position => _obj?.transform.position ?? Vector3.positiveInfinity;

        public override int GetHashCode()
        {
            return _obj.GetHashCode();
        }
    }
}
