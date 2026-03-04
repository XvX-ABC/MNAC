using MNAC.Interaction;
using UnityEngine;

namespace MNAC.Characters.UI
{
    internal class PlayerCursorIndicator : MNAC.UI.PlayerCursorIndicator
    {
        ILockTarget _lockTarget;
        IDamageable _damageable;
        public ILockTarget LockTarget
        {
            get => _lockTarget;
            set
            {
                var obj = value?.Obj;
                _damageable = obj?.GetComponent<IDamageable>();
                _lockTarget = value;
            }
        }

        void Update()
        {
            var slider = Slider_lm;
            if (_damageable != null)
            {
                if (!slider.enabled)
                {
                    slider.enabled = true;
                }
                var hp = _damageable.HP;
                slider.Value = hp.MaxPoint > 0 ? hp.Point / hp.MaxPoint : 1;
            }
            else
            {
                slider.Value = 1;
                slider.enabled = false;
            }

        }
    }
}
