using System;
using Tests.Behaviours.Arm.Weapons;
using Tests.States;
using Tests.Utilities.Timeline;
using Tests.Utilities.Timeline.Events.Point;
using Tests.Weapons_New.Sword;
using UnityEngine;
namespace Tests.Characters.Humanoid.Arms.Weapons.Sword
{
    internal class SwordSlash : ArmedArmStateBase
    {
        SlashHelper _helper;
        float _oldMultiplier;
        SwordAction _extensionAction;
        SwordAction _slashAction;

        public SwordAction ExtensionAction { get => _extensionAction; set => _extensionAction = value; }
        public SwordAction SlashAction
        {
            get => _slashAction;
            set
            {
                if (value != null)
                    value.Duration = _helper.slashDuration;
                _slashAction = value;
            }
        }

        public SwordSlash(SlashHelper slashHelper) : base("sword_slash", 0)
        {
            _helper = slashHelper ?? throw new ArgumentNullException(nameof(slashHelper));
            var timeline = _helper.state.Timeline;
            var length = timeline.Length;
            this.timeline = new Timeline(length);
            var v = length > 0 ? _helper.slashDuration / length : 0;
            this.timeline.AddPointEvent(v, _ =>
            {
                DisableSlashAction();
            });
        }
        public override void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionBegin(currentTransition);
            _helper.slashing = true;
        }
        public override void OnEnter()
        {
            base.OnEnter();
            EnableSlashActions();
            timeline.Restart();
        }
        void EnableSlashActions()
        {
            if (_extensionAction != null)
            {
                _oldMultiplier = _extensionAction.Multiplier;
                _extensionAction.Multiplier = 100f;
                _extensionAction.Enabled = true;
            }
            if (_slashAction != null)
                _slashAction.Enabled = true;
        }
        void DisableSlashAction()
        {
            if (_extensionAction != null)
            {
                _extensionAction.Multiplier = _oldMultiplier;
                _extensionAction.Enabled = false;
            }
            if (_slashAction != null)
                _slashAction.Enabled = false;
        }
        public override void OnUpdate()
        {
            base.OnUpdate();
            timeline.OnUpdate(Time.deltaTime);
        }
        public override void OnExit()
        {
            _helper.slashing = false;
            if (_extensionAction != null && _extensionAction.Enabled)
                DisableSlashAction();
            timeline.End();
            base.OnExit();
        }
    }
}
