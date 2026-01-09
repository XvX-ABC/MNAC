using RootMotion.FinalIK;
using System;
using System.Text;
using Tests.Animations;
using Tests.Characters.Humanoid.Locomotion;
using Tests.Characters.Interaction.Input;
using Tests.Interaction;
using Tests.States;
using Tests.TPhysics.Environment;
using Tests.Weapons_New.Launcher;
using UnityEngine;
using UnityEngine.Playables;
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

        ArmedLauncherArmBehaviour _behaviour;


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
          ArmedLauncherArmBehaviour ownerBehaviour,
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
            {
                _behaviour = ownerBehaviour ?? throw new ArgumentNullException(nameof(ownerBehaviour));
                _animatorController = animationDefinitions.Animator ?? throw new ArgumentNullException("animator");
                _definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
                this.animationDefinitions = animationDefinitions ?? throw new ArgumentNullException(nameof(animationDefinitions));
                _input = input ?? throw new ArgumentNullException(nameof(_input));

                _aimingHelper = new AimingHelper(aimIK, targetChangeDuration);

                _aimIK = aimIK ?? throw new ArgumentNullException(nameof(_aimIK));
            }

            {
                _controller = new(graph, _animatorController);
                //TODO：检查移动到行为的构造参数中
                var isLeft = _behaviour.BodyPart switch
                {
                    Characters.Humanoid.HumanBodyPart.LeftArm => true,
                    Characters.Humanoid.HumanBodyPart.RightArm => false,
                    _ => throw new Exception("The body part must be one of the arms.")
                };
                _controller.SetBool(this.animationDefinitions.MirrorSwitch, isLeft);
            }

            {
                idle = new Idle(
                rbody,
                world,
                groundDetector,
                _controller,
                locomotionCore.definitions.Walking.MaxSpeed,
                locomotionCore.definitions.Walking.AcceleratedSpeed,
                this.animationDefinitions.Velocity_X,
                this.animationDefinitions.Velocity_Y);


                aiming = new ArmAiming(_controller, _aimingHelper, this.animationDefinitions.Aiming);

                reload = new AmmoLoad(_controller, this.animationDefinitions.ReloadTrigger, this.animationDefinitions.ReloadMultiplier, this.animationDefinitions.ReloadClipLength);
            }
            InitializeStatemacine(_aimIK);
        }
        public IOutputSetting OutputSetting { get => _controller.OutputSetting; set => _controller.OutputSetting = value; }
        public bool Enabled
        {
            get => _aimIK.enabled;
            set
            {
                _aimIK.enabled = value;
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
            statemachine.AddTransitionFor(idle, reload, 0, _behaviour.TryReload, null, InterruptionSource.None);

            var a_r = new BlendingTransition<object>(aiming, reload, _behaviour.TryReload, null, animationDefinitions.GetStateTransitionOption(Transition.Aiming_Reload));
            statemachine.AddTransitionFor(aiming, idle, length_i_a, () => AimingTarget == null, null);
            statemachine.AddTransitionFor(a_r);

            var r_i = new BlendingTransition<object>(reload, idle, () => AimingTarget == null, null, 0, 0, 1);
            var r_a = new BlendingTransition<object>(reload, aiming, () => AimingTarget != null, null, animationDefinitions.GetStateTransitionOption(Transition.Reload_Aiming));

            statemachine.AddTransitionFor(r_i);
            statemachine.AddTransitionFor(r_a);

            state = new(this);

        }
        public void Update()
        {
            statemachine.OnUpdate();
        }
    }
}
