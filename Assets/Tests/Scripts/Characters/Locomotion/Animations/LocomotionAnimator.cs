using System;
using System.Security;
using Tests.Characters.Legs;
using Tests.Input;
using Tests.States;
using Tests.TPhysics;
using Tests.TPhysics.Environment;
using UnityEngine;

namespace Tests.Characters.Locomotion.Animations
{
    public interface ILocomotionAnimatorDefinitions
    {
        public string Velocity_X { get; }
        public string Velocity_Y { get; }
        public string Jump_Trigger { get; }

    }
    internal class Transition : BlendingTransition<LocomotionAnimationStateContext>
    {
        public Transition(IWithCallbackPlayableState<LocomotionAnimationStateContext> sourceState, IWithCallbackPlayableState<LocomotionAnimationStateContext> destinationState, Func<bool> triggerEvent, Action<IPlayableState<LocomotionAnimationStateContext>, IPlayableState<LocomotionAnimationStateContext>, float> durationEvent, float duration, float offset, float fixedExitTime = -1, InterruptionSource interruptionSource = InterruptionSource.Next) : base(sourceState, destinationState, triggerEvent, durationEvent, duration, offset, fixedExitTime, interruptionSource)
        {
        }
    }
    internal class MovementState : LocomotionAnimationStateBase
    {

        public MovementState(string name, float duration, ILocomotionAnimatorDefinitions definitions, ControllerPlayable controller, World world, Rigidbody rigidbody, IGroundDetector ground, bool enabled = true) : base(name, duration, definitions, controller, world, rigidbody, ground, enabled)
        {
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            var velocity = rbody.velocity;
            var rotation = world.rotation;

            if (groundDetector.Grounds.Count > 0)
            {
                rotation *= Quaternion.FromToRotation(world.Up, groundDetector.GroundsNormal);
            }
            velocity = Quaternion.Inverse(rotation) * velocity;

            controller.SetFloat(definitions.Velocity_X, velocity.x);
            controller.SetFloat(definitions.Velocity_Y, velocity.z);
        }
    }
    internal class GroundedMovementState : MovementState
    {
        LegsCore _legs;
        public GroundedMovementState(string name, float duration, ILocomotionAnimatorDefinitions definitions, LegsCore legs, ControllerPlayable controller, World world, Rigidbody rigidbody, IGroundDetector ground, bool enabled = true) : base(name, duration, definitions, controller, world, rigidbody, ground, enabled)
        {
            _legs = legs ?? throw new ArgumentNullException(nameof(legs));
        }
        public override void FromPreviousStateTransitionRunning(IReadonlyPlayableTransition<LocomotionAnimationStateContext> currentTransition)
        {
            base.FromPreviousStateTransitionRunning(currentTransition);
            _legs.Weight = currentTransition.Timeline.NormalizedTime;
        }
        public override void ToNextStateTransitionRunning(IReadonlyPlayableTransition<LocomotionAnimationStateContext> currentTransition)
        {
            base.ToNextStateTransitionRunning(currentTransition);
            _legs.Weight = 1 - currentTransition.Timeline.NormalizedTime;
        }
    }
    internal class JumpState : LocomotionAnimationStateBase
    {
        public JumpState(float duration, ILocomotionAnimatorDefinitions definitions, ControllerPlayable controller, World world, Rigidbody rigidbody, IGroundDetector ground, bool enabled = true) : base("jump", duration, definitions, controller, world, rigidbody, ground, enabled)
        {
        }
        public override void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<LocomotionAnimationStateContext> currentTransition)
        {
            base.FromPreviousStateTransitionBegin(currentTransition);
            controller.SetTrigger(definitions.Jump_Trigger);
        }
    }

    internal class LocomotionAnimator : CharacterComponentBase_MonoComponent
    {
        ILocomotionAnimatorDefinitions _definitions;
        IGroundDetector groundDetector;
        IInput _input;
        LocomotionCore _core;

        LocomotionAnimationStateContext _context;
        LocomotionAnimationStatemachine _statemachine;

        MovementState _movementState;
        GroundedMovementState _groundedMovementState;
        JumpState _jumpState;

        public override string Name => $"{base.Name}_animator";
        protected override void Awake()
        {
            base.Awake();
            _definitions = GetComponent<ILocomotionAnimatorDefinitions>() ?? throw new ComponentCantFindException(this.gameObject, typeof(ILocomotionAnimatorDefinitions));

        }
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            if (!blackboard.TryReadValue<IInput>(CharacterBlackboardFields.Input, out _input))
                throw new Exception();
            if (!blackboard.TryReadValue<ControllerPlayable>(CharacterBlackboardFields.Character_Animator_Main, out var controller))
                throw new Exception();
            if (!blackboard.TryReadValue<LocomotionCore>(CharacterBlackboardFields.Character_Locomotion_Core, out _core))
                throw new Exception();
            if (!blackboard.TryReadValue<Rigidbody>(CharacterBlackboardFields.Rigidbody, out var rbody))
                throw new Exception();
            if (!blackboard.TryReadValue<World>(CharacterBlackboardFields.World, out var world))
                throw new Exception();
            if (!blackboard.TryReadValue<IGroundDetector>(CharacterBlackboardFields.GroundDetector, out var grouondDetectors))
                throw new Exception();
            if (!blackboard.TryReadValue<LegsCore>(CharacterBlackboardFields.Character_Legs_Core, out var legs))
                throw new Exception();

            InitializeStates(legs, controller, world, rbody, grouondDetectors);
            InitializeStatemachine();
        }
        void InitializeStates(LegsCore legs, ControllerPlayable controller, World world, Rigidbody rigidbody, IGroundDetector groundDetector)
        {
            _movementState = new(null, 0, _definitions, controller, world, rigidbody, groundDetector);
            _groundedMovementState = new(null, 0, _definitions, legs, controller, world, rigidbody, groundDetector);
            _jumpState = new(0, _definitions, controller, world, rigidbody, groundDetector);
        }
        void InitializeStatemachine()
        {
            _context = new();
            _statemachine = new("main", _context);
            _statemachine.AddState(_groundedMovementState);
            _statemachine.AddState(_movementState);
            _statemachine.AddState(_jumpState);

            var gm_m = new Transition(_groundedMovementState, _movementState, () => groundDetector.Grounds.Count == 0, null, 1, 0, 0);
            //var gm_j = new Transition(_groundedMovementState, _jumpState, () => _input.Jump, null, 0, 0, 0);

            var m_gm = new Transition(_movementState, _groundedMovementState, () => groundDetector.Grounds.Count > 0, null, 1, 0, 0);

            _statemachine.AddTransitionFor(gm_m);
            _statemachine.AddTransitionFor(m_gm);
        }
        public void FixedUpdate()
        {
            _statemachine.OnUpdate();
        }
    }
}
