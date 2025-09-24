using System;
using System.Collections.Generic;
using Tests.Behaviours.Arms.Weapons.Launchers.Animations;
using Tests.Input;
using Tests.Interaction;
using Tests.States;
using Tests.Weapons;
using Tests.Weapons.Launcher;

namespace Tests.Behaviours.Arms.Weapons.Launchers
{
    internal class ArmedLauncherArmBehaviour : ArmedWeaponArmBehaviourBase
    {
        IArmedLauncherArmBehaviourDefinitions _definitions;
        ITargetsCatcher _targetsCatcher;
        IInput _input;
        ILauncher _launcher;
        internal ITarget target;


        internal Idle idle;
        internal ArmAiming aiming;
        internal BodyBehaviour.Arm.Weapons.Launcher.AmmoLoad ammoLoad;
        internal WithCallbackPlayableStatemachine<object> statemachine;
        ArmedLauncherArmBehaviourState _state;

        internal ArmedLauncherArmAnimator animator;

        public ArmedLauncherArmBehaviour(IArmedLauncherArmBehaviourDefinitions definitions, ArmedLauncherArmAnimator animator)
        {
            _definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
            this.animator = animator ?? throw new ArgumentNullException(nameof(animator));
            InitializeStatemachine();
        }

        public override WeaponType Type => WeaponType.Launcher;

        public override IWeapon Weapon
        {
            get => _launcher;
            set
            {
                if (value is ILauncher launcher)
                {
                    _launcher = launcher;
                    ammoLoad.TargetLauncher = launcher;
                    animator.Launcher = launcher;
                    aiming.ControlledWeapon = launcher;
                }
                else
                    throw new Exception("Weapon");
            }
        }

        public override IArmedWeaponArmAnimationPlayablePart Animator => animator;

        public override Func<bool> EntryFunc => () => this.enabled;

        public override Func<bool> ExitFunc => () => !this.enabled;

        public IInput Input
        {
            get => _input;
            set
            {
                aiming.Input = value;
                _input = value;
            }
        }
        public ITargetsCatcher TargetsCatcher
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
                _targetsCatcher = value;
            }
        }

        public override IWithCallbackPlayableState<object> State => _state;

        void WhenTargetsChanged(IList<ITarget> targets)
        {
            target = targets.Count > 0 ? targets[^1] : null;
            animator.AimingTarget = target;
        }
        void InitializeStatemachine()
        {
            statemachine = new("armed_launcher_statemachine");
            idle = new Idle();
            aiming = new ArmAiming();
            ammoLoad = new BodyBehaviour.Arm.Weapons.Launcher.AmmoLoad();

            statemachine.AddState(idle);
            statemachine.AddState(aiming);
            statemachine.AddState(ammoLoad);

            var length = _definitions.AimAndReloadTransitionLength;

            statemachine.AddTransitionFor(idle, aiming, length, () => target != null, null);
            statemachine.AddTransitionFor(idle, ammoLoad, 0, ReloadTriggered, null, InterruptionSource.None);


            statemachine.AddTransitionFor(aiming, idle, length, () => target == null, null);
            statemachine.AddTransitionFor(aiming, ammoLoad, length, ReloadTriggered, null, InterruptionSource.None);

            var l_i = new BlendingTransition<object>(ammoLoad, idle, () => target == null, null, 0, 0, 1);
            var l_a = new BlendingTransition<object>(ammoLoad, aiming, () => target != null, null, length, 0, 0.75f);
            statemachine.AddTransitionFor(l_i);
            statemachine.AddTransitionFor(l_a);

            _state = new(this);

            bool WeaponCanToReload()
            {
                return _launcher.Definitions.AmmoInMagazineQuantity > _launcher.MagazineAmmoCount && _launcher.ReservesAmmoCount > 0;
            }
            bool ReloadTriggered()
            {
                return _input == null ? false : _input.Reload && WeaponCanToReload();
            }
        }
        public void FixedUpdate()
        {
            animator.Update();
        }
    }
}
