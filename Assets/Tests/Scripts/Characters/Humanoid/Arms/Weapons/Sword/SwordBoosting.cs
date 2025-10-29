using System;
using Tests.Behaviours.Arm.Weapons;
using Tests.States;
using UnityEngine;
using Utilities.Timeline;
namespace Tests.Characters.Humanoid.Arms.Weapons.Sword
{
    internal class SwordBoosting : ArmedArmStateBase
    {
        BoostingHelper _helper;
        public SwordBoosting(BoostingHelper boostingHelper) : base("sword_boosting", 0)
        {
            _helper = boostingHelper ?? throw new ArgumentNullException(nameof(boostingHelper));
            var sbTimeline = _helper.state.Timeline;
            timeline = new Timeline_V1(sbTimeline.Length);
        }
        public override void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionBegin(currentTransition);
            _helper.inBoosting = true;
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
            _helper.inBoosting = false;
            base.OnExit();
        }
    }
}
