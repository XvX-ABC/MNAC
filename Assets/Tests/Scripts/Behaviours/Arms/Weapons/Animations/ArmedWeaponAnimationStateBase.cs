using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Animations;
using Tests.States;
using Utilities.Timeline;

namespace Tests.Behaviours.Arms.Weapon.Animations
{
    internal class ArmedWeaponAnimationStateBase : WithCallbackPlayableState<object>
    {
        //public ArmedWeaponAnimationStateBase(ControllerPlayable controller, string name, float duration = 0, bool enabled = true) : base(name == null ? "armed _weapon_animation" : $"armed_{name}_animation", duration, enabled)
        //{
        //    this.controller = controller ?? throw new ArgumentNullException(nameof(controller));
        //}
        public ArmedWeaponAnimationStateBase(string name, float duration = 0, bool enabled = true) : base(name == null ? "armed _weapon_animation" : $"armed_{name}_animation", duration, enabled)
        {

        }
        protected override ITimeline NewTimeline(float duration)
        {
            return new Timeline_V1(duration);
        }

    }
}
