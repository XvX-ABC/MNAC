using RootMotion.FinalIK;
using System;
using System.Collections.Generic;
using System.Text;
using Tests.Animations;
using Tests.Characters.Humanoid.Locomotion;
using Tests.Characters.Interaction.Input;
using Tests.Input;
using Tests.Interaction;
using Tests.States;
using Tests.TPhysics.Environment;
using Tests.Weapons.Launcher;
using Tests.Weapons_New.Launcher;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SocialPlatforms;
using static Tests.Behaviours.Arms.Weapons.Launcher.Animations.IArmedLauncherArmAnimationDefinitions;
using World = Tests.TPhysics.World;

namespace Tests.Behaviours.Arms.Weapons.Launcher.Animations
{
    internal class ArmedLauncherArmAnimator : IArmedWeaponArmAnimationPlayablePart
    {
        ControllerPlayable _controller;
        RuntimeAnimatorController _animatorController;
        IArmedLauncherArmBehaviourDefinitions _definitions;
        internal IArmedLauncherArmAnimationDefinitions animationDefinitions;
        AimIK _aimIK;

        IWeaponControlInput _input;
        ITargetsCatcher _targetsCatcher;
        ILauncher _launcher;


        AimingHelper _aimingHelper;


        internal Idle idle;
        internal ArmAiming aiming;
        internal AmmoLoad reload;
        internal WithCallbackPlayableStatemachine<object> statemachine;
        internal ArmedLauncherAnimationState state;
        [Obsolete]
        public ArmedLauncherArmAnimator(
            PlayableGraph graph,
            AimIK aimIK,
            Rigidbody rbody,
            World world,
            IGroundDetector groundDetector,
            LocomotionCore locomotionCore,
            IArmedLauncherArmBehaviourDefinitions definitions,
            IArmedLauncherArmAnimationDefinitions animationDefinitions,
            ITargetsCatcher targetsCatcher,
            IWeaponControlInput input)
        {
            _animatorController = animationDefinitions.Animator ?? throw new ArgumentNullException("animator");
            _definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
            this.animationDefinitions = animationDefinitions ?? throw new ArgumentNullException(nameof(animationDefinitions));
            _targetsCatcher = targetsCatcher ?? throw new ArgumentNullException(nameof(targetsCatcher));
            _input = input ?? throw new ArgumentNullException(nameof(_input));

            _aimingHelper = new AimingHelper(aimIK, 0);

            _aimIK = aimIK ?? throw new ArgumentNullException(nameof(_aimIK));

            _controller = new(graph, _animatorController);

            idle = new Idle(rbody, world, groundDetector, _controller, locomotionCore.definitions.Walking.MaxSpeed, locomotionCore.definitions.Walking.AcceleratedSpeed, this.animationDefinitions.Velocity_X, this.animationDefinitions.Velocity_Y);


            aiming = new ArmAiming(_controller, _aimingHelper, this.animationDefinitions.Aiming);

            reload = new AmmoLoad(_controller, this.animationDefinitions.ReloadTrigger, this.animationDefinitions.ReloadMultiplier, this.animationDefinitions.ReloadClipLength);
            InitializeStatemacine(_aimIK);
        }

        public ArmedLauncherArmAnimator(
          PlayableGraph graph,
          AimIK aimIK,
          Rigidbody rbody,
          World world,
          IGroundDetector groundDetector,
          LocomotionCore locomotionCore,
          float targetChangeDuration,
          IArmedLauncherArmBehaviourDefinitions definitions,
          IArmedLauncherArmAnimationDefinitions animationDefinitions,
          IWeaponControlInput input)
        {
            _animatorController = animationDefinitions.Animator ?? throw new ArgumentNullException("animator");
            _definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
            this.animationDefinitions = animationDefinitions ?? throw new ArgumentNullException(nameof(animationDefinitions));
            _input = input ?? throw new ArgumentNullException(nameof(_input));

            _aimingHelper = new AimingHelper(aimIK, targetChangeDuration);

            _aimIK = aimIK ?? throw new ArgumentNullException(nameof(_aimIK));

            _controller = new(graph, _animatorController);

            idle = new Idle(rbody, world, groundDetector, _controller, locomotionCore.definitions.Walking.MaxSpeed, locomotionCore.definitions.Walking.AcceleratedSpeed, this.animationDefinitions.Velocity_X, this.animationDefinitions.Velocity_Y);


            aiming = new ArmAiming(_controller, _aimingHelper, this.animationDefinitions.Aiming);

            reload = new AmmoLoad(_controller, this.animationDefinitions.ReloadTrigger, this.animationDefinitions.ReloadMultiplier, this.animationDefinitions.ReloadClipLength);
            InitializeStatemacine(_aimIK);
        }
        public IOutputSetting OutputSetting { get => _controller.OutputSetting; set => _controller.OutputSetting = value; }
        public bool Enabled
        {
            get => _aimIK.enabled;
            set
            {
                //_aimIK.enabled = value;
                _aimIK.enabled = value;
                /*
                 * DONE：从其他状态到此状态的过渡开始时，动画会产生意外的扭曲行为
                 * 因为此处提前将权重设置为0/1，混合器中的权重值不正确，导致动画在过渡时产生扭曲
                 * _controller.OutputSetting.Weight = value ? 1 : 0;
                 */
            }
        }
        public ILockTarget AimingTarget { get => _aimingHelper.Target; set => _aimingHelper.Target = value; }
        public ILauncher Launcher
        {
            get => _launcher;
            set
            {
                reload.TargetLauncher = value;
                _launcher = value;
                _aimingHelper.ControlledWeapon = value;
            }
        }
        public Playable GetPlayablePart(PlayableGraph graph)
        {
            return _controller.PlayablePart;


        }
        void InitializeStatemacine(AimIK aimIK)
        {



            statemachine = new("armed_launcher_animation_statemachine");
            statemachine.AddState(idle);
            statemachine.AddState(aiming);
            statemachine.AddState(reload);


            var length_i_a = animationDefinitions.GetStateTransitionOption(Transition.Idle_Aiming).Duration;

            statemachine.AddTransitionFor(idle, aiming, length_i_a, () => AimingTarget != null, null);
            statemachine.AddTransitionFor(idle, reload, 0, TriggeredReload, null, InterruptionSource.None);

            var a_r = new BlendingTransition<object>(aiming, reload, TriggeredReload, null, animationDefinitions.GetStateTransitionOption(Transition.Aiming_Reload));
            statemachine.AddTransitionFor(aiming, idle, length_i_a, () => AimingTarget == null, null);
            statemachine.AddTransitionFor(a_r);

            var r_i = new BlendingTransition<object>(reload, idle, () => AimingTarget == null, null, 0, 0, 1);
            var r_a = new BlendingTransition<object>(reload, aiming, () => AimingTarget != null, null, animationDefinitions.GetStateTransitionOption(Transition.Reload_Aiming));

            statemachine.AddTransitionFor(r_i);
            statemachine.AddTransitionFor(r_a);

            state = new(this);

            bool TriggeredReload() => _input == null ? false : _input.Reload && _launcher.Definitions.AmmoInMagazineAmount > _launcher.MagazineAmmoAmount && _launcher.ReserveAmmoAmount > 0;
        }
        [Obsolete]
        void InitializeStatemacine_Obsolete(AimIK aimIK)
        {


            var length = 1;


            statemachine = new("armed_launcher_statemachine");
            statemachine.AddState(idle);
            statemachine.AddState(aiming);
            statemachine.AddState(reload);


            statemachine.AddTransitionFor(idle, aiming, length, () => AimingTarget != null, null);
            statemachine.AddTransitionFor(idle, reload, 0, TriggeredReload, null, InterruptionSource.None);

            var a_r = new BlendingTransition<object>(aiming, reload, TriggeredReload, null, length, 0, 1, InterruptionSource.None);
            statemachine.AddTransitionFor(aiming, idle, length, () => AimingTarget == null, null);
            statemachine.AddTransitionFor(a_r);

            var r_i = new BlendingTransition<object>(reload, idle, () => AimingTarget == null, null, 0, 0, 1);
            var r_a = new BlendingTransition<object>(reload, aiming, () => AimingTarget != null, null, 0.25f, 0, 0.75f);

            statemachine.AddTransitionFor(r_i);
            statemachine.AddTransitionFor(r_a);

            state = new(this);

            bool TriggeredReload() => _input == null ? false : _input.Reload && _launcher.Definitions.AmmoInMagazineAmount > _launcher.MagazineAmmoAmount && _launcher.ReserveAmmoAmount > 0;
        }
        public void Update()
        {
            statemachine.OnUpdate();
            //Debug.Log(statemachine);
            var sb = new StringBuilder();
            sb.AppendLine(statemachine.ToString());
            //Debug.Log(sb.ToString());
        }
    }
}
