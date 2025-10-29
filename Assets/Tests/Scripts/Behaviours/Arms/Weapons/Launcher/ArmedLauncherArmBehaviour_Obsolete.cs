using RootMotion.FinalIK;
using System;
using System.Collections.Generic;
using Tests.BodyBehaviour.Arm.Weapons.Launcher;
using Tests.Input;
using Tests.Interaction;
using Tests.States;
using Tests.Weapons;
using Tests.Weapons.Launcher;
using UnityEngine;
using ArmAim = Tests.Behaviours.Arms.ArmAim_Obsolete;

namespace Tests.Behaviours.Arms.Weapons.Launchers
{
    [Obsolete]
    public class ArmedLauncherArmBehaviour_Obsolete : ArmedWeaponArmBehaviourBase_Obsolete
    {
        ILauncher _launcher;
        PlayableStateMachine _stateMachine;
        ArmAim_Obsolete _aim;
        AmmoLoad_Obsolete _ammoLoad;
        ITargetsCatcher _targetsCatcher;
        IArmedLauncherArmBehaviourDefinitions _definitions;
        IInput_Obsolete _input;

        internal ArmedLauncherArmAnimator_Obsolete animator;

        public override IWeapon Weapon
        {
            get => _launcher;
            set
            {
                if (value is ILauncher launcher)
                {
                    _launcher = launcher;
                    _ammoLoad.Launcher = launcher;
                    animator.reload.ReloadTimeline = launcher.ReloadTimeline;
                    _aim.Weapon = _launcher;
                }
                else
                    throw new Exception("Weapon");
            }
        }
        public override IArmedWeaponArmAnimationPlayablePart Animator { get => animator; }


        public override Func<bool> EntryFunc { get => Enter; }
        public override Func<bool> ExitFunc { get => Exit; }
        public IInput_Obsolete Input
        {
            get => _input;
            set
            {
                _aim.Input = value;
                _input = value;
            }
        }
        internal ITargetsCatcher targetsCatcher
        {
            get => _targetsCatcher;
            set
            {
                if (_targetsCatcher != null)
                {
                    _targetsCatcher.TargetsChangedAction -= TargetsChanged;
                }
                if (value != null)
                    value.TargetsChangedAction += TargetsChanged;
                _targetsCatcher = value;
            }
        }


        public override WeaponType Type => WeaponType.Launcher;
        public override bool Activated
        {
            get => base.Activated;
            set
            {
                base.Activated = value;
                _aim.Enabled = value;
            }
        }
        protected internal ArmedLauncherArmBehaviour_Obsolete(IArmedLauncherArmBehaviourDefinitions definitions, AimIK aimIK, ITargetsCatcher targetsCatcher) : base("launcher")
        {
            this._definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
            this._aim = new(aimIK ?? throw new ArgumentNullException(nameof(aimIK)));
            this.targetsCatcher = targetsCatcher ?? throw new ArgumentNullException(nameof(targetsCatcher));

            animator = new(_definitions, _aim);
            InitializeAmmoReload();
            InitializeStateMachine();

            _aim.weightChangedAction += v =>
            {
                animator.AimingWeight = v;
            };

        }
        void TargetsChanged(IList<ITarget> targets)
        {
            var target = targets.Count > 0 ? targets[^1] : null;
            _aim.Target = target;
        }
        void InitializeAmmoReload()
        {
            _ammoLoad = new(animator.reload);
            _ammoLoad.ExitWhenEnd = true;
        }
        void InitializeStateMachine()
        {
            _stateMachine = new("armed_launcher_statemachine");
            _stateMachine.AddState(_aim);
            _stateMachine.AddState(_ammoLoad);

            _stateMachine.AddTransitionFor(_aim, _ammoLoad, _definitions.AimAndReloadTransitionLength, () => _input == null ? false : _input.Reload, (s, d, t) =>
            {
                (s as ArmAim_Obsolete).Weight = 1 - t;
            });


            _stateMachine.AddTransitionFor(_ammoLoad, _aim, _definitions.AimAndReloadTransitionLength, (s, d, t) =>
            {
                (d as ArmAim_Obsolete).Weight = t;
            });
        }
        protected virtual bool Enter()
        {
            var targets = _targetsCatcher.Targets;
            return targets.Count > 0;
        }
        protected virtual bool Exit()
        {
            return _stateMachine.CurrentState == _aim && _targetsCatcher.Targets.Count == 0;
        }
        public override void OnEnter()
        {
            _stateMachine?.OnEnter();
        }

        public override void OnExit()
        {
            _stateMachine?.OnExit();
        }

        public override void OnUpdate()
        {
            _stateMachine?.OnUpdate();
        }
        float v0;
        float v1;
        public override void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionBegin(currentTransition);
            _stateMachine.ChangeStateTo(_aim);
            v0 = animator.IdleWeight;
            v1 = _aim.Weight;
        }
        public override void FromPreviousStateTransitionRunning(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionRunning(currentTransition);
            var v = currentTransition.Timeline.NormalizedTime;
            if (_stateMachine.CurrentState == _aim)
            {
                _aim.Weight = Mathf.Lerp(v1, 1, v);
                _aim.ToNextStateTransitionRunning(currentTransition);
            }
            animator.IdleWeight = Mathf.Lerp(v0, 0, v);
        }



        public override void ToNextStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.ToNextStateTransitionBegin(currentTransition);
            v0 = animator.IdleWeight;
            v1 = _aim.Weight;
        }
        public override void ToNextStateTransitionRunning(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.ToNextStateTransitionRunning(currentTransition);
            var v = currentTransition.Timeline.NormalizedTime;
            if (_stateMachine.CurrentState == _aim)
            {
                _aim.Weight = Mathf.Lerp(v1, 0, v);
                _aim.ToNextStateTransitionRunning(currentTransition);
            }
            animator.IdleWeight = Mathf.Lerp(v0, 1, v);
        }
        public override void ToNextStateTransitionEnd(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.ToNextStateTransitionEnd(currentTransition);
            _stateMachine.OnExit();
        }

    }
}

