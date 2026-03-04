using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MNAC.Animations;
using MNAC.States;
using MNAC.Utilities.Timeline;

namespace MNAC.Behaviours.Arms.Weapon.Animations
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
            return new Timeline(duration);
        }

    }
}
