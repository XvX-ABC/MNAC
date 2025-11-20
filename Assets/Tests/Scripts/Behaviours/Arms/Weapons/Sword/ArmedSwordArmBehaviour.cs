using System;
using System.Collections.Generic;
using Tests.Behaviours.Arms.Weapons.Sword.Animations;
using Tests.Input;
using Tests.Interaction;
using Tests.Interaction.Targets;
using Tests.States;
using Tests.TPhysics.Locomotion;
using Tests.Weapons;
using UnityEngine;

namespace Tests.Behaviours.Arms.Weapons.Sword
{
    internal class ArmedSwordArmBehaviour : ArmedWeaponArmBehaviourBase
    {
        IArmedSwordArmBehaviourDefinitions _definitions;
        ISword _sword;
        ISphereTriggerTargetsCatcher _targetsCatcher;

        Interaction.ITarget_Obsolete _target;

        internal Idle idle;
        internal BoostingHelper boostingHelper;
        internal Boosting boosting;
        internal SlashHelper slashHelper;
        internal Slash slash;
        internal WithCallbackPlayableStatemachine<object> statemachine;

        BoostingLocomotion _boostingLocomotion;

        internal ArmedSwordArmAnimator animator;
        ArmedSwordArmBehaviourState _state;
        public override WeaponType Type => WeaponType.Sword;

        public override IWeapon_Obsolete Weapon
        {
            get => _sword;
            set
            {
                if (value is ISword sword)
                {
                    _sword = sword;
                    if (_targetsCatcher != null)
                        _targetsCatcher.Radius = _sword.SlashRadius * 0.5f;
                    slash.Sword = _sword;
                }
                else
                    throw new Exception("Weapon");
            }
        }

        public override IArmedWeaponArmAnimationPlayablePart Animator => animator;

        public override Func<bool> EntryFunc => () => this.enabled;

        public override Func<bool> ExitFunc => () => !this.enabled;

        public override IWithCallbackPlayableState<object> State => _state;

        public ISphereTriggerTargetsCatcher TargetsCatcher
        {
            get => _targetsCatcher;
            set
            {
                if (_targetsCatcher != null)
                {
                    _targetsCatcher.TargetsChangedAction -= WhenTargetsChanged;
                }
                if (value != null)
                    value.TargetsChangedAction += WhenTargetsChanged;
                value.Radius = _sword == null ? 0 : _sword.SlashRadius;
                _targetsCatcher = value;

                boosting.TargetsCatcher = _targetsCatcher;
            }
        }
        public override bool Activated
        {
            get => base.Activated;
            set
            {
                base.Activated = value;
                _targetsCatcher.Enabled = value;
                statemachine.Enabled = value;
                animator.Enabled = true;
            }
        }
        public ArmedSwordArmBehaviour(IArmedSwordArmBehaviourDefinitions definitions, BoostingHelper boostingHelper, SlashHelper slashHelper, ArmedSwordArmAnimator animator)
        {
            _definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));

            this.animator = animator ?? throw new ArgumentNullException(nameof(animator));

            InitializeStates(boostingHelper, slashHelper);

            InitializeStatemachine();

        }
        void InitializeStates(BoostingHelper boostingHelper, SlashHelper slashHelper)
        {
            idle = new();
            this.boostingHelper = boostingHelper ?? throw new ArgumentNullException(nameof(boostingHelper));
            boosting = this.boostingHelper.state;
            this.slashHelper = slashHelper ?? throw new ArgumentNullException(nameof(slashHelper));
            slash = this.slashHelper.state;
        }
        void InitializeStatemachine()
        {
            statemachine = new("armed_sword_statemachine");
            statemachine.AddState(idle);
            statemachine.AddState(boosting);
            statemachine.AddState(slash);


            statemachine.AddTransitionFor(idle, boosting, () => boostingHelper.EntryEvent);

            var b_s = new BlendingTransition<object>(boosting, slash, () => slashHelper.EntryEvent, null, 0);
            statemachine.AddTransitionFor(b_s);

            var b_i = new BlendingTransition<object>(boosting, idle, () => boostingHelper.ExitEvent, null, 0, 0, BlendingTransition<object>.FIXED_EXIT_TIME_INVALID_VALUE, InterruptionSource.None);
            statemachine.AddTransitionFor(b_i);


            var s_i = new BlendingTransition<object>(slash, idle, () => slashHelper.ExitEvent, null, 0, 0, -1, InterruptionSource.None);
            statemachine.AddTransitionFor(s_i);

            _state = new(this);
        }
        void WhenTargetsChanged(IList<Interaction.ITarget_Obsolete> targets)
        {
            slashHelper.Target = targets.Count > 0 ? targets[^1] : null;
        }
        public override void Update()
        {
            boostingHelper.Update();
            animator.Update();
        }
    }
}
