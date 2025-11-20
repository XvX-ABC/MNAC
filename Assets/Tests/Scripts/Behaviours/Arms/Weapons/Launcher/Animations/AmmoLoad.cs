using System;
using Tests.Animations;
using Tests.States;
using Tests.Weapons.Launcher;
using UnityEngine;

namespace Tests.Behaviours.Arms.Weapons.Launcher.Animations
{
    internal class AmmoLoad : ArmedLauncherAnimationStateBase
    {
        ILauncher_Obsolete _launcher;
        string _animationName;
        string _multiplierName;
        float _clipLength;
        public AmmoLoad(ControllerPlayable controller, string animationTrigger, string animationMultiplier, float clipLength, bool enabled = true) : base(controller, "ammo_load", 0, enabled)
        {
            _animationName = animationTrigger ?? throw new ArgumentNullException(nameof(animationTrigger));
            _multiplierName = animationMultiplier ?? throw new ArgumentNullException(nameof(animationMultiplier));
            _clipLength = Mathf.Max(0, clipLength);
        }
        internal ILauncher_Obsolete TargetLauncher
        {
            get => _launcher;
            set
            {
                var length = value == null ? 0 : value.ReloadTimeline.Length;
                timeline.UpdateLength(length);
                var v = _clipLength / (length <= 0 ? 1 : length); ;
                controller.SetFloat(_multiplierName, v);
                _launcher = value;
            }
        }
        public override void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionBegin(currentTransition);

            //controller.SetTrigger(_trigger);
            controller.SetBool(_animationName, true);
        }
        public override void OnEnter()
        {
            base.OnEnter();
            timeline.Restart();
        }
        public override void OnUpdate()
        {
            base.OnUpdate();
            timeline.OnUpdate(Time.deltaTime);
        }
        public override void OnExit()
        {
            timeline.End();
            controller.SetBool(_animationName, false);
            base.OnExit();
        }
    }
}
