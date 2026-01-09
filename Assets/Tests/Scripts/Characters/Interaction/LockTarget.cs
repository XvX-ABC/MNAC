using System;
using Tests.Characters.Humanoid;
using Tests.Interaction;
using Tests.UI;
using UnityEngine;
using IndicatedTarget = Tests.Characters.UI.IndicatedTarget;
using ULockType = Tests.UI.BoxIndicator.LockType;
namespace Tests.Characters.Interaction
{
    internal class LockTarget : MonoBehaviour, ILockTarget
    {
        LockType _lockType;
        GameObject _chestObj;
        Vector3 _positionCatche;
        IndicatedTarget _indicatedTarget;
        Action<LockTarget> _disableCallback;
        IDamageable _damageable;
        internal BoxIndicator indicator => (BoxIndicator)_indicatedTarget?.Indicator;
        internal IndicatedTarget indicatedTarget
        {
            get => _indicatedTarget;
            set
            {
                if (value != null)
                    value.IndicatedObj = _chestObj ?? this.gameObject;
                _indicatedTarget = value;
            }
        }
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


        public GameObject Obj => this.gameObject;

        public Vector3 Position
        {
            get
            {
                var pos = _positionCatche;
                if (this != null)
                {
                    if (_chestObj != null)
                    {
                        pos = _chestObj.transform.position;
                    }
                    else if (this.gameObject != null)
                    {
                        pos = this.transform.position;
                    }
                }
                _positionCatche = pos;
                return pos;
            }
        }
        internal Action<LockTarget> DisableCallback { get => _disableCallback; set => _disableCallback = value; }
        internal GameObject chestObj
        {
            get => _chestObj;
            set
            {
                if (_indicatedTarget != null && value != null)
                    _indicatedTarget.IndicatedObj = value;
                _chestObj = value;
            }
        }

        void Awake()
        {
            if (TryGetComponent<ICompositeItems>(out var compositeItems))
            {
                chestObj = compositeItems.GetItem((uint)HumanBodyPart.Chest);
            }
            _damageable = GetComponent<IDamageable>();
        }
        void Update()
        {
            this.enabled = _damageable.HP.IsAlive;
        }
        void OnDisable()
        {
            _disableCallback?.Invoke(this);
        }
    }
}
