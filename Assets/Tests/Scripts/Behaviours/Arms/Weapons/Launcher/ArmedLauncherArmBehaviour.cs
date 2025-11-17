using System;
using System.Collections.Generic;
using Tests.Behaviours.Arms.Weapons.Launcher.Animations;
using Tests.Characters.Interaction.Input;
using Tests.Input;
using Tests.Interaction;
using Tests.States;
using Tests.Weapons;
using Tests.Weapons.Launcher;
using TMPro;
using UnityEngine;

namespace Tests.Behaviours.Arms.Weapons.Launcher
{
    internal class ArmedLauncherArmBehaviour : ArmedWeaponArmBehaviourBase
    {
        IArmedLauncherArmBehaviourDefinitions _definitions;
        [Obsolete]
        ITargetsCatcher _targetsCatcher;
        TargetLocker _targetLocker;
        IWeaponControlInput _winput;
        ILauncher _launcher;
        internal Interaction.ITarget_Obsolete target;


        internal Idle idle;
        internal ArmAiming aiming;
        internal AmmoLoad ammoLoad;
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
        public IWeaponControlInput Input
        {
            get => _winput;
            set
            {
                aiming.Input = value;
                _winput = value;
            }
        }
        //[Obsolete]
        //public ITargetsCatcher TargetsCatcher
        //{
        //    get => _targetsCatcher;
        //    set
        //    {
        //        if (_targetsCatcher != null)
        //        {
        //            _targetsCatcher.TargetsChangedAction -= WhenTargetsChanged;
        //        }
        //        if (value != null)
        //            value.TargetsChangedAction += WhenTargetsChanged;
        //        _targetsCatcher = value;
        //    }
        //}
        public TargetLocker TargetLocker
        {
            get => _targetLocker;
            set
            {
                if (_targetLocker != null)
                    _targetLocker.MainObjTargetChangedAction -= WhenTargetChanged;
                if (value != null)
                    value.MainObjTargetChangedAction += WhenTargetChanged;
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
        //[Obsolete]
        //void WhenTargetsChanged(IList<Interaction.ITarget_Obsolete> targets)
        //{
        //    target = targets.Count > 0 ? targets[^1] : null;
        //    animator.AimingTarget = target;
        //}
        void WhenTargetChanged(IGameObjTarget _, IGameObjTarget newTarget)
        {
            animator.AimingTarget = newTarget;
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

            //FIXME: 装弹状态没有正常退出
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
                //return _input == null ? false : _input.Reload && WeaponCanToReload();
                return _winput == null ? false : _winput.Reload && WeaponCanToReload();
            }
        }
        public override void Update()
        {
            animator.Update();
        }
    }
}
