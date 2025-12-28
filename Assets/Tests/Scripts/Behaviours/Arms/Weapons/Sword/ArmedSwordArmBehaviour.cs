using System;
using System.Collections.Generic;
using Tests.Behaviours.Arms.Weapons.Sword.Animations;
using Tests.Interaction;
using Tests.States;
using Tests.TPhysics.Locomotion;
using Tests.Weapons_New;
using Tests.Weapons_New.Sword;
using UnityEngine;
using WeaponType = Tests.Weapons_New.WeaponType;

namespace Tests.Behaviours.Arms.Weapons.Sword
{
    internal class ArmedSwordArmBehaviour : ArmedWeaponArmBehaviourBase
    {
        IArmedSwordArmBehaviourDefinitions _definitions;
        ISword _sword;
        [Obsolete]
        PlayerTargetLocker _targetLocker;
        ISphericalObjsTrigger _targetsTrigger;
        LayerMask _layerMaskToHit;

        [Obsolete]
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

        public override IWeapon Weapon
        {
            get => _sword;
            set
            {
                if (value is ISword sword)
                {
                    _sword = sword;
                    if (_targetsTrigger != null)
                        _targetsTrigger.Radius = _sword.Length;
                    _sword.LayerMaskToHit = _layerMaskToHit;

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
        public ISphericalObjsTrigger TargetsTrigger
        {
            get => _targetsTrigger;
            set
            {
                if (_targetsTrigger != null)
                {
                    _targetsTrigger.CaughtItemsChangedAction -= WhenTargetsChanged;
                }
                if (value != null)
                {
                    value.CaughtItemsChangedAction += WhenTargetsChanged;
                    value.Radius = _sword?.Length ?? 0;
                }
                _targetsTrigger = value;

                boosting.TargetsTrigger = _targetsTrigger;
            }
        }
        public override bool Activated
        {
            get => base.Activated;
            set
            {
                base.Activated = value;
                _targetsTrigger.Enabled = value;
                statemachine.Enabled = value;
                animator.Enabled = true;
            }
        }

        public LayerMask LayerMaskToHit
        {
            get => _layerMaskToHit;
            set
            {
                if (_sword != null)
                    _sword.LayerMaskToHit = value;
                _layerMaskToHit = value;
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
        void WhenTargetsChanged(IList<GameObject> targets)
        {
            slashHelper.Target = targets.Count > 0 ? targets[^1] : null;
        }
        public override void BehaviourOnUpdate()
        {
            Debug.Log(statemachine);
            boostingHelper.Update();
            animator.Update();

        }
    }
}
