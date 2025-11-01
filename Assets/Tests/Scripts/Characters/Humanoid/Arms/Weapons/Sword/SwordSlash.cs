using System;
using Tests.Behaviours.Arm.Weapons;
using Tests.States;
using Tests.Utilities.Timeline;
using UnityEngine;
namespace Tests.Characters.Humanoid.Arms.Weapons.Sword
{
    internal class SwordSlash : ArmedArmStateBase
    {
        SlashHelper _helper;
        public SwordSlash(SlashHelper slashHelper) : base("sword_slash", 0)
        {
            _helper = slashHelper ?? throw new ArgumentNullException(nameof(slashHelper));
            var timeline = _helper.state.Timeline;
            this.timeline = new Timeline_V1(timeline.Length);
        }
        public override void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionBegin(currentTransition);
            _helper.slashing = true;
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
            _helper.slashing = false;
            timeline.End();
            base.OnExit();
        }
    }
}
