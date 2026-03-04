using System;
using System.Reflection;
using MNAC.Animations;
using MNAC.States;
using UnityEngine;
using UnityEngine.Animations;

namespace MNAC.Behaviours.Arms.Weapons.Sword.Animations
{
    internal class Boosting : ArmedSwordWholeBodyAnimationStateBase
    {
        string _switchName;
        string _multiplierName;
        float _clipLength;
        ArmControllerPlayable _armController;
        public Boosting(ControllerPlayable baseWholeBodyController, WholeBodyControllerPlayable wholeBodyController, ArmControllerPlayable armController, float duration = 0, bool enabled = true) : base(baseWholeBodyController, wholeBodyController, "boosting", duration, enabled)
        {
            _armController = armController;
        }
        void UpdateSpeedMultiplier()
        {
            //var m = _clipLength / (timeline.Length <= 0 ? 1 : timeline.Length);
            //controller.SetFloat(_multiplierName, m);
            wholeBodyController.SetBoostingMultiplier(timeline.Length);
        }


        public override void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionBegin(currentTransition);
            UpdateSpeedMultiplier();
            _armController.OutputSetting.Weight = 0;
            //controller.SetBool(_switchName, true);
            wholeBodyController.SetBoostingSwitch(true);
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
            wholeBodyController.SetBoostingSwitch(false);
            timeline.End();
            _armController.OutputSetting.Weight = 1;
            base.OnExit();
        }
    }
}
