using System;
using UnityEngine;
using UnityEngine.UI;

namespace MNAC.UI
{
    [ExecuteAlways]
    public class BoxIndicator : Indicator, IIndicator
    {
        #region static area
        static UIComponentPool<BoxIndicator> s_pool;
        public static void InitializeObjPool(BoxIndicator prototype, Transform parent = null)
        {
            s_pool = new(prototype, parent);
        }
        public static BoxIndicator GetInstance()
        {
            if (s_pool == null)
                throw new Exception();
            return s_pool.Get();
        }
        public static void ReleaseInstance(BoxIndicator instance)
        {
            s_pool.Release(instance);
        }
        #endregion
        [SerializeField]
        Color _unlockColor;
        [SerializeField]
        Color _unconfirmColor;
        [SerializeField]
        Color _confirmedColor;
        [SerializeField]
        LockType _type;
        Image _image;

        public LockType Type
        {
            get => _type;
            set
            {
                _type = value;
                _image.color = _type switch
                {
                    LockType.Unlock => _unlockColor,
                    LockType.Lock_Unconfirm => _unconfirmColor,
                    LockType.Lock_Confirmed => _confirmedColor,
                    _ => throw new NotImplementedException()
                };
            }
        }
        public enum LockType
        {
            Unlock,
            Lock_Unconfirm,
            Lock_Confirmed,
        }
        protected override void Awake()
        {
            base.Awake();
            _image = GetComponentInChildren<Image>();
            Type = Type;
        }

    }
}
