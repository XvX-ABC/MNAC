using System;

namespace MNAC.Behaviours.Arms.Weapons.Animations
{
    internal interface IArmedArmAnimator
    {
        public ArmedWeaponPlayablePart PlayablePart { get; }
        [Obsolete]
        void OnUpdate();
    }
}
