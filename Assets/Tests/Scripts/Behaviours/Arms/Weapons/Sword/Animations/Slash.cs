using System;
using System.Threading;
using Tests.Animations;
using Tests.States;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

namespace Tests.Behaviours.Arms.Weapons.Sword.Animations
{
    internal class Slash : ArmedSwordWholeBodyAnimationStateBase
    {
        ArmControllerPlayable _armController;
        float _duration;
        float _recovery;
        public Slash(ControllerPlayable baseWholeBodyController, WholeBodyControllerPlayable wholeBodyController, ArmControllerPlayable armController, float duration = 0, float recovery = 0, bool enabled = true) : base(baseWholeBodyController, wholeBodyController, "slash", duration, enabled)
        {
            _armController = armController;
            _duration = duration;
            _recovery = recovery;
            timeline.UpdateLength(duration + recovery);
        }
        void UpdateSpeedMultiplier()
        {
            //var m = _clipLength / (timeline.Length > 0 ? timeline.Length : 1);
            //controller.SetFloat(_multiplierName, m);
            //wholeBodyController.SetSlashMultiplier(timeline.Length);
            wholeBodyController.SetSlashMultiplier(_duration);
        }
        public override void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionBegin(currentTransition);
            UpdateSpeedMultiplier();
            _armController.OutputSetting.Weight = 0;
            //controller.SetBool(_switchName, true);
            wholeBodyController.SetSlashSwitch(true);
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
            //controller.SetBool(_switchName, false);
            wholeBodyController.SetSlashSwitch(false);
            timeline.End();
            _armController.OutputSetting.Weight = 1;
            base.OnExit();
        }
    }
}
