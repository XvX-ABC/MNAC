using System;
using System.Reflection;
using Tests.Animations;
using Tests.States;
using UnityEngine;
using UnityEngine.Animations;

namespace Tests.Behaviours.Arms.Weapons.Sword.Animations
{
    internal class Boosting : ArmedSwordWholeBodyAnimationStateBase
    {
        string _switchName;
        string _multiplierName;
        float _clipLength;
        ControllerPlayable _armController;
        public Boosting(ControllerPlayable controller, ControllerPlayable armController, MixerPlayable mixer, string switchName, string multiplierName, float clipLength, float duration = 0, bool enabled = true) : base(mixer, controller, "boosting", duration, enabled)
        {
            _switchName = switchName ?? throw new ArgumentNullException(nameof(switchName));
            _multiplierName = multiplierName ?? throw new ArgumentNullException(nameof(multiplierName));
            _clipLength = Mathf.Max(0, clipLength);
            _armController = armController;
        }
        void UpdateSpeedMultiplier()
        {
            var m = _clipLength / (timeline.Length <= 0 ? 1 : timeline.Length);
            controller.SetFloat(_multiplierName, m);
        }


        public override void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionBegin(currentTransition);
            UpdateSpeedMultiplier();
            _armController.outputSetting.Weight = 0;
            controller.SetBool(_switchName, true);
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
            controller.SetBool(_switchName, false);
            timeline.End();
            _armController.outputSetting.Weight = 1;
            base.OnExit();
        }
    }
}
