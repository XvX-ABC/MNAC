using Tests.Characters.Humanoid;
using Tests.Characters.UI;
using Tests.Interaction;
using Tests.UI;
using UnityEngine;
using UnityEngine.Pool;
using IndicatedTarget = Tests.Characters.UI.IndicatedTarget;
using ULockType = Tests.UI.BoxIndicator.LockType;
namespace Tests.Characters.Interaction
{
    internal class LockTarget : ILockTarget
    {
        static ObjectPool<LockTarget> s_pool;
        static GameObject _currentObj;
        static GameObject _currentChestObj;
        static LockType _currentType;
        static LockTarget()
        {
            s_pool = new(CreateInstance, WhenGetInstance, WhenReleaseInstance, DestroyInstance, false, 5, 1000);
        }
        public static LockTarget GetInstance(GameObject obj, LockType type)
        {
            if (obj.TryGetComponent<ICompositeItems>(out var compositeItems))
            {
                _currentChestObj = compositeItems.GetItem((uint)HumanBodyPart.Chest);
            }

            _currentObj = obj;
            _currentType = type;
            var result = s_pool.Get();
            _currentObj = null;
            _currentChestObj = null;
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
            target.ChestObj = _currentChestObj;
            target.LockType = _currentType;
        }
        static void WhenReleaseInstance(LockTarget target)
        {
            target.Obj = null;
            target.ChestObj = null;
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
        GameObject _chestObj;
        LockType _lockType;
        IndicatedTarget _indicatedTarget;
        internal BoxIndicator indicator => (BoxIndicator)_indicatedTarget?.Indicator;
        internal IndicatedTarget indicatedTarget
        {
            get => _indicatedTarget;
            set
            {
                if (value != null)
                    value.IndicatedObj = _chestObj;
                _indicatedTarget = value;
            }
        }
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

        public Vector3 Position => _chestObj?.transform?.position ?? _obj.transform.position;

        public GameObject ChestObj { get => _chestObj; set => _chestObj = value; }

        public override int GetHashCode()
        {
            return _obj.GetHashCode();
        }
    }
}
