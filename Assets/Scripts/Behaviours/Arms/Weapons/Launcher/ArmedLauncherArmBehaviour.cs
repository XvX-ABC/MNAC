using System;
using Tests.Behaviours.Arms.Weapons.Launcher.Animations;
using Tests.Characters.Humanoid;
using Tests.Characters.Interaction.Input;
using Tests.Interaction;
using Tests.States;
using Tests.Weapons_New;
using Tests.Weapons_New.Launcher;
using UnityEngine;
using WeaponType = Tests.Weapons_New.WeaponType;

namespace Tests.Behaviours.Arms.Weapons.Launcher
{
    internal class ArmedLauncherArmBehaviour : ArmedWeaponArmBehaviourBase
    {
        IArmedLauncherArmBehaviourDefinitions _definitions;
        ITargetLocker _targetLocker;
        IWeaponControlInput _winput;
        ILauncher _launcher;
        ILauncherDefinitions _launcherDefinitions;
        internal IGameObjTarget_New target;
        HumanBodyPart _part;


        internal Idle idle;
        internal ArmAiming aiming;
        internal AmmoLoad ammoLoad;
        internal WithCallbackPlayableStatemachine<object> statemachine;
        ArmedLauncherArmBehaviourState _state;

        internal ArmedLauncherArmAnimator animator;

        internal LayerMask layerMaskToHit;
        internal TeamMask teamMask;


        public ArmedLauncherArmBehaviour(HumanBodyPart part, IArmedLauncherArmBehaviourDefinitions definitions)
        {
            _part = part;
            _definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
            TeamMask = _definitions.TeamMask;
            LayerMaskToHit = _definitions.LayerMaskToHit;
            //InitializeStatemachine();
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
                    _launcherDefinitions = _launcher.Definitions;
                    ammoLoad.TargetLauncher = launcher;
                    animator.Launcher = launcher;
                    aiming.ControlledWeapon = launcher;

                }
                else
                    throw new Exception("Weapon");
            }
        }

        public override IArmedWeaponArmAnimationPlayablePart Animator => animator;

        public override Func<bool> ActivationTrigger => () => this.enabled;

        public override Func<bool> UnactivationTrigger => () => !this.enabled;
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
                {
                    value.MainTargetChangedAction += WhenTargetChanged;
                    UpdateTarget(value.MainLockTarget);
                }
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

        public HumanBodyPart BodyPart { get => _part; }

        void WhenTargetChanged(ILockTarget _, ILockTarget newTarget)
        {
            UpdateTarget(newTarget);
        }
        internal void InitializeStatemachine()
        {
            statemachine = new("armed_launcher_statemachine");
            idle = new Idle();
            aiming = new ArmAiming();
            ammoLoad = new AmmoLoad();

            statemachine.AddState(idle);
            statemachine.AddState(aiming);
            statemachine.AddState(ammoLoad);

            var length = 0;

            statemachine.AddTransitionFor(idle, aiming, length, () => target != null, null);
            statemachine.AddTransitionFor(idle, ammoLoad, 0, TryReload, null, InterruptionSource.None);


            statemachine.AddTransitionFor(aiming, idle, length, () => target == null, null);
            statemachine.AddTransitionFor(aiming, ammoLoad, length, TryReload, null, InterruptionSource.None);

            var l_i = new BlendingTransition<object>(ammoLoad, idle, () => target == null, null, 0, 0, 1);
            var l_a = new BlendingTransition<object>(ammoLoad, aiming, () => target != null, null, animator.animationDefinitions.GetStateTransitionOption(IArmedLauncherArmAnimationDefinitions.Transition.Reload_Aiming));
            statemachine.AddTransitionFor(l_i);
            statemachine.AddTransitionFor(l_a);

            _state = new(this);

        }
        internal bool TryReload()
        {
            return TryAutoReload() || TryManualReload();
            bool TryAutoReload()
            {
                return _launcherDefinitions.AllowedAutoReload && _launcher.MagazineAmmoAmount == 0;
            }
            bool TryManualReload()
            {
                return _winput == null ? false : _winput.Reload /*&& WeaponCanToReload()*/;
            }

        }
        void UpdateTarget(ILockTarget newTarget)
        {
            if (animator != null)
                animator.AimingTarget = newTarget;
            target = newTarget;
            //Debug.Log($"The armed launcher arm will  change the aiming target to '{newTarget}'");
        }
        public override void BehaviourOnUpdate()
        {
            animator.Update();
        }
    }
}
