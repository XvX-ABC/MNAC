using System;
using System.Threading;
using Tests.Animations;
using Tests.Behaviours.Arms.Weapon.Animations;

namespace Tests.Behaviours.Arms.Weapons.Sword.Animations
{
    internal class ArmedSwordAnimationStateBase : ArmedWeaponAnimationStateBase
    {
        protected ControllerPlayable controller;
        internal ArmedSwordAnimationStateBase(ControllerPlayable controller, string name, float duration = 0, bool enabled = true) : base(name == null ? "sword" : $"sword_{name}", duration, enabled)
        {
            this.controller = controller ?? throw new ArgumentNullException(nameof(controller));
        }

    }
}
