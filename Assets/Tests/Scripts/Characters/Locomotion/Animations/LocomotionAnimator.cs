using System;
using System.Security;
using Tests.Animations;
using Tests.Characters.Legs;
using Tests.Input;
using Tests.States;
using Tests.TPhysics;
using Tests.TPhysics.Environment;
using Tests.TPhysics.Locomotion;
using Tests.Utilities.Composable;
using UnityEditor.Rendering;
using UnityEngine;

namespace Tests.Characters.Locomotion.Animations
{

    internal class LocomotionAnimator : ComponentBase
    {
        ILocomotionAnimatorDefinitions _definitions;
        IGroundDetector groundDetector;
        IInput _input;
        LocomotionCore _core;

        LocomotionAnimationStateContext _context;
        internal LocomotionAnimationStatemachine statemachine;

        QuickBoostingHelper _quickBoostingHelper;
        internal QuickBoostingState quickBoosting;
        //InAirMovementState _airMovementState;
        internal GroundedMovementState groundedMovement;
        internal DescendingState descending;
        internal JumpState jump;

        public override string Name => $"locomotion_animator";
        public LocomotionAnimator(ILocomotionAnimatorDefinitions definitions, LocomotionCore core)
        {
            _definitions = definitions ?? throw new ArgumentNullException(nameof(definitions));
            _core = core;
        }
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            if (!blackboard.TryReadValue<IInput>(CharacterBlackboardFields.Character_Input_Main, out _input))
                throw new Exception();
            if (!blackboard.TryReadValue<ControllerPlayable>(CharacterBlackboardFields.Character_Animation_Animator, out var controller))
                throw new Exception();
            if (!blackboard.TryReadValue<Rigidbody>(CharacterBlackboardFields.Rigidbody, out var rbody))
                throw new Exception();
            if (!blackboard.TryReadValue<World>(CharacterBlackboardFields.World, out var world))
                throw new Exception();
            if (!blackboard.TryReadValue<IGroundDetector>(CharacterBlackboardFields.GroundDetector, out groundDetector))
                throw new Exception();
            if (!blackboard.TryReadValue<LegsCore>(CharacterBlackboardFields.Character_Legs_Core, out var legs))
                throw new Exception();

            InitializeStates(legs, controller, world, rbody, groundDetector);
            InitializeStatemachine();
        }
        void InitializeStates(LegsCore legs, ControllerPlayable controller, World world, Rigidbody rigidbody, IGroundDetector groundDetector)
        {
            var d = _core.definitions;
            {
                var maxSpeed = d.Walking.MaxSpeed;
                var animator = new MovementAnimator(_definitions, maxSpeed, d.Walking.AcceleratedSpeed, rigidbody, world, groundDetector, controller);
                //_airMovementState = new(null, 0, animator);
                groundedMovement = new(null, 0, legs, animator);
            }

            {
                var timeline = _core.jump.Timeline;

                jump = new(_definitions, controller, timeline);

                descending = new(null, _definitions, controller);
            }

            {
                _quickBoostingHelper = _core.quickBoostingHelper;
                var timeline = _core.quickBoosting.Timeline;
                var qbDefinitions = d.Boosting;
                var maxSpeed = d.Walking.MaxSpeed * qbDefinitions.MaxSpeedPower;
                var qbAnimator = new MovementAnimator(_definitions, maxSpeed, maxSpeed, rigidbody, world, groundDetector, controller);
                quickBoosting = new(null, timeline, _definitions, controller, qbAnimator);
            }

        }
        void InitializeStatemachine()
        {
            _context = new();
            statemachine = new("main", _context);
            statemachine.AddState(groundedMovement);
            statemachine.AddState(jump);
            statemachine.AddState(descending);
            statemachine.AddState(quickBoosting);

            var gm_j = new BindLocomotionTransition(groundedMovement, jump, () => groundDetector.Grounds.Count > 0 && _input.Jump, null, 0);
            var gm_d = new Transition(groundedMovement, descending, () => groundDetector.Grounds.Count == 0 && _core.locomotionContext.VerticalPosture == VerticalPosture.Descending, null, 0);
            var gm_qb = new BindLocomotionTransition(groundedMovement, quickBoosting, () => _quickBoostingHelper.TriggerEvent, null, 0);

            var j_d = new Transition(jump, descending, () => _core.locomotionContext.VerticalPosture == VerticalPosture.Descending, null, 0, 0, 1);
            var j_qb = new BindLocomotionTransition(jump, quickBoosting, () => _quickBoostingHelper.TriggerEvent, null, 0);

            var d_gm = new Transition(descending, groundedMovement, () => groundDetector.Grounds.Count > 0, null, 0);
            var d_qb = new BindLocomotionTransition(descending, quickBoosting, () => _quickBoostingHelper.TriggerEvent, null, 0);

            var qb_gm = new Transition(quickBoosting, groundedMovement, () => groundDetector.Grounds.Count > 0, null, 0, 0, 1, InterruptionSource.None);
            var qb_d = new Transition(quickBoosting, descending, () => _core.locomotionContext.VerticalPosture == VerticalPosture.Descending, null, 0, 0, 1, InterruptionSource.None);

            statemachine.AddTransitionFor(gm_j);
            statemachine.AddTransitionFor(gm_d);
            statemachine.AddTransitionFor(gm_qb);

            statemachine.AddTransitionFor(j_d);
            statemachine.AddTransitionFor(j_qb);

            statemachine.AddTransitionFor(d_gm);
            statemachine.AddTransitionFor(d_qb);

            statemachine.AddTransitionFor(qb_gm);
            statemachine.AddTransitionFor(qb_d);
        }
    }
}
