using System;
using MNAC.Characters.Humanoid;
using MNAC.Interaction;
using MNAC.UI;
using UnityEngine;
using UnityEngine.Pool;
using IndicatedTarget = MNAC.Characters.UI.IndicatedTarget;
using ULockType = MNAC.UI.BoxIndicator.LockType;
namespace MNAC.Characters.Interaction
{
    [Obsolete]
    internal class LockTarget_Obsolete : ILockTarget
    {
        static ObjectPool<LockTarget_Obsolete> s_pool;
        static GameObject _currentObj;
        static GameObject _currentChestObj;
        static LockType _currentType;
        static LockTarget_Obsolete()
        {
            s_pool = new(CreateInstance, WhenGetInstance, WhenReleaseInstance, DestroyInstance, false, 5, 1000);
        }
        public static LockTarget_Obsolete GetInstance(GameObject obj, LockType type)
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
        public static void ReleaseInstance(LockTarget_Obsolete target)
        {
            s_pool.Release(target);
        }
        static LockTarget_Obsolete CreateInstance()
        {
            return new();
        }
        static void WhenGetInstance(LockTarget_Obsolete target)
        {
            target.Obj = _currentObj;
            target.ChestObj = _currentChestObj;
            target.LockType = _currentType;
        }
        static void WhenReleaseInstance(LockTarget_Obsolete target)
        {
            target.Obj = null;
            target.ChestObj = null;
            target.LockType = LockType.None;
        }
        static void DestroyInstance(LockTarget_Obsolete target)
        {

        }
        public static explicit operator GameObject(LockTarget_Obsolete target)
        {
            return target._obj;
        }
        GameObject _obj;
        GameObject _chestObj;
        LockType _lockType;
        IndicatedTarget _indicatedTarget;
        Vector3 _positionCatche;
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

        //public Vector3 Position => _chestObj?.transform?.position ?? _obj.transform.position;
        public Vector3 Position
        {
            get
            {
                var pos = _positionCatche;
                if (_chestObj != null)
                    pos = _chestObj.transform.position;
                else if (_obj != null)
                    pos = _obj.transform.position;
                _positionCatche = pos;
                return pos;
            }
        }

        public GameObject ChestObj { get => _chestObj; set => _chestObj = value; }

        public override int GetHashCode()
        {
            return _obj.GetHashCode();
        }
    }
}
