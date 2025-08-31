using RootMotion.FinalIK;
using System;
using System.Collections.Generic;
using Tests.Behaviours.Arms.Animations;
using Tests.BodyBehaviour.Arm.Weapons.Launcher;
using Tests.Characters;
using Tests.Characters.Arms.Weapons.Launchers;
using Tests.Input;
using Tests.States;
using Tests.Weapons;
using Tests.Weapons.Launcher;
using ArmAim = Tests.BodyBehaviour.Arm.Weapons.Launcher.ArmAim;

namespace Tests.Behaviours.Arms.Weapons.Launchers
{
    public class ArmedLauncherArmBehaviour : ArmedWeaponArmBehaviourBase
    {
        ILauncher _launcher;
        PlayableStateMachine _stateMachine;
        ArmAim _aim;
        AmmoLoad _ammoLoad;
        internal ArmedLauncherArmAnimator banimator;
        ITargetsCatcher _targetsCatcher;
        IArmedLauncherArmBehaviourDefinitions _definitions;
        IInput _input;
        public override IWeapon Weapon
        {
            get => _launcher;
            set
            {
                if (value is ILauncher launcher)
                {
                    _launcher = launcher;
                    _ammoLoad.Launcher = launcher;
                    banimator.reload.ReloadTimeline = launcher.ReloadTimeline;
                }
                else
                    throw new Exception("Weapon");
            }
        }
        public override IArmedWeaponArmAnimationPlayablePart Animator { get => banimator; }


        public override Func<bool> EntryFunc { get => Enter; }
        public override Func<bool> ExitFunc { get => Exit; }
        public IInput Input
        {
            get => _input;
            set => _input = value;
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
        protected internal ArmedLauncherArmBehaviour(IArmedLauncherArmBehaviourDefinitions definitions, AimIK aimIK, ITargetsCatcher targetsCatcher) : base("launcher")
        {
            this._definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
            this._aim = new(aimIK ?? throw new ArgumentNullException(nameof(aimIK)));
            this.targetsCatcher = targetsCatcher ?? throw new ArgumentNullException(nameof(targetsCatcher));

            banimator = new(_definitions, _aim);

            InitializeAmmoReload();
            InitializeStateMachine();

            _aim.weightChangedAction += v =>
            {
                banimator.AimingWeight = v;
            };

        }
        void TargetsChanged(IList<ITarget> targets)
        {
            var target = targets.Count > 0 ? targets[0] : null;
            _aim.Target = target;
        }
        void InitializeAmmoReload()
        {
            _ammoLoad = new(banimator.reload);
            _ammoLoad.ExitWhenEnd = true;
        }
        void InitializeStateMachine()
        {
            _stateMachine = new("armed_launcher_statemachine");
            _stateMachine.AddState(_aim);
            _stateMachine.AddState(_ammoLoad);

            _stateMachine.AddTransitionFor(_aim, _ammoLoad, _definitions.AimAndReloadTransitionLength, () => _input == null ? false : _input.Reload, (s, d, t) =>
            {
                if (statusNum != 2)
                    statusNum = 2;
                (s as ArmAim).Weight = 1 - t;
            });


            _stateMachine.AddTransitionFor(_ammoLoad, _aim, _definitions.AimAndReloadTransitionLength, (s, d, t) =>
            {
                if (statusNum != 1)
                    statusNum = 1;
                (d as ArmAim).Weight = t;
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
        public override void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionBegin(currentTransition);
            _stateMachine.ChangeStateTo(_aim);
        }
        public override void FromPreviousStateTransitionRunning(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionRunning(currentTransition);
            var v = currentTransition.Timeline.NormalizedTime;
            if (_stateMachine.CurrentState == _aim)
            {
                _aim.Weight = v;
                _aim.ToNextStateTransitionRunning(currentTransition);
            }
            banimator.IdleWeight = 1 - v;
        }
        public override void ToNextStateTransitionRunning(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.ToNextStateTransitionRunning(currentTransition);
            var v = currentTransition.Timeline.NormalizedTime;
            if (_stateMachine.CurrentState == _aim)
            {
                _aim.Weight = 1 - v;
                _aim.ToNextStateTransitionRunning(currentTransition);
            }
            banimator.IdleWeight = v;
        }
        public override void ToNextStateTransitionEnd(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.ToNextStateTransitionEnd(currentTransition);
            _stateMachine.OnExit();
        }

    }
}

