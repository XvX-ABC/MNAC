using System;
using UnityEngine;
using static Tests.UI.BoxIndicator;

namespace Tests.UI
{
    public class Target_Debug : IndicatedTarget
    {
        [SerializeField]
        [Range(0, 100)]
        float _health;
        BoxIndicator _indicator;
        public override Indicator Indicator
        {
            get => base.Indicator;
            set
            {
                base.Indicator = value;
                if (value != null)
                    _indicator = base.indicator as BoxIndicator ?? throw new InvalidCastException("Indicator must be a BoxIndicator");
            }
        }
        private void Update()
        {
            if (_indicator != null)
            {

                var t = _health switch
                {
                    < 50 => LockType.Unlock,
                    >= 50 and < 75 => LockType.Lock_Unconfirm,
                    >= 75 => LockType.Lock_Confirmed,
                };
                _indicator.Type = t;
            }
        }

    }
}
