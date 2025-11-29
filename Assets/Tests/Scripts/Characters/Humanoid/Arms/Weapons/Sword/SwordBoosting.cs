using System;
using Tests.Behaviours.Arm.Weapons;
using Tests.States;
using Tests.Utilities.Timeline;
using Tests.Weapons_New.Sword;
using UnityEngine;
namespace Tests.Characters.Humanoid.Arms.Weapons.Sword
{
    internal class SwordBoosting : ArmedArmStateBase
    {
        ISword _sword;
        BoostingHelper _helper;
        SwordAction _extensionAction;
        ArmedArmStateBase _followingSlashState;

        internal ArmedArmStateBase FollowingSlashState { get => _followingSlashState; set => _followingSlashState = value; }
        public SwordAction ExtensionAction { get => _extensionAction; set => _extensionAction = value; }

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
            if (_extensionAction != null )
                _extensionAction.Enabled = true;
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
            _extensionAction.Enabled = false;
            base.OnExit();
        }
    }
}
