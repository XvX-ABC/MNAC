using RootMotion.FinalIK;
using System;
using System.Collections.Generic;
using Tests.Animations;
using Tests.Characters;
using Tests.Characters.Locomotion;
using Tests.Input;
using Tests.Interaction;
using Tests.States;
using Tests.TPhysics.Environment;
using Tests.Weapons.Launcher;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using World = Tests.TPhysics.World;

namespace Tests.Behaviours.Arms.Weapons.Launchers.Animations
{
    internal class ArmedLauncherArmAnimator : IArmedWeaponArmAnimationPlayablePart
    {
        ControllerPlayable _controller;
        RuntimeAnimatorController _animatorController;
        IArmedLauncherArmBehaviourDefinitions _definitions;
        IArmedLauncherArmAnimationDefinitions _animationDefinitions;
        AimIK _aimIK;

        IInput _input;
        ITargetsCatcher _targetsCatcher;
        ILauncher _launcher;


        AimingHelper _aimingHelper;


        Idle idle;
        ArmAiming aiming;
        AmmoLoad reload;
        WithCallbackPlayableStatemachine<object> _statemachine;
        internal ArmedLauncherAnimationState state;
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
            IInput input)
        {
            _animatorController = animationDefinitions.Animator ?? throw new ArgumentNullException("animator");
            _definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
            _animationDefinitions = animationDefinitions ?? throw new ArgumentNullException(nameof(animationDefinitions));
            _targetsCatcher = targetsCatcher ?? throw new ArgumentNullException(nameof(targetsCatcher));
            _input = input ?? throw new ArgumentNullException(nameof(input));

            _aimingHelper = new AimingHelper(aimIK);

            _aimIK = aimIK ?? throw new ArgumentNullException(nameof(_aimIK));

            _controller = new(graph, _animatorController);

            idle = new Idle(rbody, world, groundDetector, _controller, locomotionCore.definitions.Walking.MaxSpeed, locomotionCore.definitions.Walking.AcceleratedSpeed, _animationDefinitions.Velocity_X, _animationDefinitions.Velocity_Y);


            aiming = new ArmAiming(_controller, _aimingHelper, _animationDefinitions.Aiming);

            reload = new AmmoLoad(_controller, _animationDefinitions.ReloadTrigger, _animationDefinitions.ReloadMultiplier, _animationDefinitions.ReloadClipLength);
            InitializeStatemacine(_aimIK);
        }
        public IOutputSetting OutputSetting { get => _controller.OutputSetting; set => _controller.OutputSetting = value; }
        public bool Enabled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ITarget AimingTarget { get => _aimingHelper.Target; set => _aimingHelper.Target = value; }
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
            //if (_controller.PlayablePart.IsNull())
            //{
            //    _controller.Initialize(graph);

            //}
            return _controller.PlayablePart;


        }
        void WhenTargetsChanged(IList<ITarget> targets)
        {
            _aimingHelper.Target = targets.Count > 0 ? targets[^1] : null;
        }

        void InitializeStatemacine(AimIK aimIK)
        {


            var length = _definitions.AimAndReloadTransitionLength;


            _statemachine = new("armed_launcher_statemachine");
            _statemachine.AddState(idle);
            _statemachine.AddState(aiming);
            _statemachine.AddState(reload);


            _statemachine.AddTransitionFor(idle, aiming, length, () => AimingTarget != null, null);
            _statemachine.AddTransitionFor(idle, reload, 0, TriggeredReload, null, InterruptionSource.None);

            var a_r = new BlendingTransition<object>(aiming, reload, TriggeredReload, null, length, 0, 1, InterruptionSource.None);
            _statemachine.AddTransitionFor(aiming, idle, length, () => AimingTarget == null, null);
            //_statemachine.AddTransitionFor(aiming, reload, length, TriggeredReload, null, InterruptionSource.None);
            _statemachine.AddTransitionFor(a_r);

            var r_i = new BlendingTransition<object>(reload, idle, () => AimingTarget == null, null, 0, 0, 1);
            var r_a = new BlendingTransition<object>(reload, aiming, () => AimingTarget != null, null, 1, 0, 0.75f);

            _statemachine.AddTransitionFor(r_i);
            _statemachine.AddTransitionFor(r_a);

            state = new(_statemachine);

            bool TriggeredReload() => _input == null ? false : _input.Reload;
        }
        public void Update()
        {
            _statemachine.OnUpdate();
            //Debug.Log(_statemachine);
        }
    }
}
