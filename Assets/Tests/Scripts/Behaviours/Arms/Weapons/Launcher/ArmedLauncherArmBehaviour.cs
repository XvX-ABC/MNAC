using System;
using Tests.Behaviours.Arms.Weapons.Launcher.Animations;
using Tests.Characters.Interaction.Input;
using Tests.Interaction;
using Tests.States;
using Tests.Weapons_New;
using Tests.Weapons_New.Launcher;
using Tests.Weapons_New.Projectiles;
using UnityEngine;
using WeaponType = Tests.Weapons_New.WeaponType;

namespace Tests.Behaviours.Arms.Weapons.Launcher
{
    internal class ArmedLauncherArmBehaviour : ArmedWeaponArmBehaviourBase
    {
        IArmedLauncherArmBehaviourDefinitions _definitions;
        [Obsolete]
        ITargetsCatcher _targetsCatcher;
        ITargetLocker _targetLocker;
        IWeaponControlInput _winput;
        ILauncher _launcher;
        internal IGameObjTarget_New target;


        internal Idle idle;
        internal ArmAiming aiming;
        internal AmmoLoad ammoLoad;
        internal WithCallbackPlayableStatemachine<object> statemachine;
        ArmedLauncherArmBehaviourState _state;

        internal ArmedLauncherArmAnimator animator;

        internal LayerMask layerMaskToHit;
        internal TeamMask teamMask;


        public ArmedLauncherArmBehaviour(IArmedLauncherArmBehaviourDefinitions definitions, ArmedLauncherArmAnimator animator)
        {
            _definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
            this.animator = animator ?? throw new ArgumentNullException(nameof(animator));
            TeamMask = _definitions.TeamMask;
            LayerMaskToHit = _definitions.LayerMaskToHit;
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
                    _launcher.TeamMask = teamMask;
                    _launcher.LayerMaskToHit = layerMaskToHit;
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
        public IWeaponControlInput Input
        {
            get => _winput;
            set
            {
                aiming.Input = value;
                _winput = value;
            }
        }
        public ITargetLocker TargetLocker
        {
            get => _targetLocker;
            set
            {
                if (_targetLocker != null)
                    _targetLocker.MainTargetChangedAction -= WhenTargetChanged;
                if (value != null)
                    value.MainTargetChangedAction += WhenTargetChanged;
                _targetLocker = value;
            }
        }
        public override IWithCallbackPlayableState<object> State => _state;
        public override bool Activated
        {
            get => base.Activated;
            set
            {
                base.Activated = value;
                //_targetsCatcher.Enabled = value;
                statemachine.Enabled = value;
                animator.Enabled = value;
            }
        }

        public LayerMask LayerMaskToHit
        {
            get => layerMaskToHit;
            set
            {
                if (_launcher != null)
                    _launcher.LayerMaskToHit = value;
                layerMaskToHit = value;
            }
        }
        public TeamMask TeamMask
        {
            get => teamMask;
            set
            {
                if (_launcher != null)
                    _launcher.TeamMask = value;
                teamMask = value;
            }
        }

        void WhenTargetChanged(ILockTarget _, ILockTarget newTarget)
        {
            animator.AimingTarget = newTarget;
            target = newTarget;
        }
        void InitializeStatemachine()
        {
            statemachine = new("armed_launcher_statemachine");
            idle = new Idle();
            aiming = new ArmAiming();
            ammoLoad = new AmmoLoad();

            statemachine.AddState(idle);
            statemachine.AddState(aiming);
            statemachine.AddState(ammoLoad);

            var length = _definitions.AimAndReloadTransitionLength;

            statemachine.AddTransitionFor(idle, aiming, length, () => target != null, null);
            statemachine.AddTransitionFor(idle, ammoLoad, 0, ReloadTriggered, null, InterruptionSource.None);


            statemachine.AddTransitionFor(aiming, idle, length, () => target == null, null);
            statemachine.AddTransitionFor(aiming, ammoLoad, length, ReloadTriggered, null, InterruptionSource.None);

            var l_i = new BlendingTransition<object>(ammoLoad, idle, () => target == null, null, 0, 0, 1);
            var l_a = new BlendingTransition<object>(ammoLoad, aiming, () => target != null, null, length, 0, 1);
            statemachine.AddTransitionFor(l_i);
            statemachine.AddTransitionFor(l_a);

            _state = new(this);

            bool WeaponCanToReload()
            {
                return _launcher.Definitions.AmmoInMagazineAmount > _launcher.MagazineAmmoAmount && _launcher.ReserveAmmoAmount > 0;
            }
            bool ReloadTriggered()
            {
                return _winput == null ? false : _winput.Reload && WeaponCanToReload();
            }
        }
        public override void Update()
        {
            animator.Update();
        }
    }
}
