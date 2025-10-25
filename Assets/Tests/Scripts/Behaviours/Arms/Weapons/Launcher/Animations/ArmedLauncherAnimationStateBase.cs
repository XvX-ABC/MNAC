using System;
using Tests.Animations;
using Tests.States;
using Utilities.Timeline;

namespace Tests.Behaviours.Arms.Weapons.Launchers.Animations
{
    //TOOD: 修改父类为ArmedWeaponAnimationStateBase
    internal class ArmedLauncherAnimationStateBase : WithCallbackPlayableState<object>
    {
        protected ControllerPlayable controller;

        public ArmedLauncherAnimationStateBase(ControllerPlayable controller, string name, float duration = 0, bool enabled = true) : base(name == null ? "armed_launcher_animation" : $"armed_launcher_animation_{name}", duration, enabled)
        {
            this.controller = controller ?? throw new ArgumentNullException(nameof(controller));
        }
        protected override ITimeline NewTimeline(float duration)
        {
            return new Timeline_V1(duration);
        }
    }
}
