using System;
using Tests.Extensions;
using Tests.Input;
using Tests.States;
using Tests.TPhysics;
using Tests.TPhysics.Environment;
using Tests.TPhysics.Locomotion;
using UnityEngine;
using Core = Tests.TPhysics.Locomotion.LocomotionCore;
namespace Tests.Characters.Locomotion
{

    internal class LocomotionCore : CharacterComponentBase_MonoComponent
    {
        ILocomotionDefinitions _definitions;

        IInput _input;

        Core _core;
        LocomotionStatemachine _movementStatemachine;
        LocomotionStatemachine _statemachine;
        LocomotionStateContext _context;

        internal QuickBoostingHelper quickBoostingHelper;

        internal BoostingState boosting;
        internal QuickBoostingState quickBoosting;
        internal WalkingState walking;
        internal JumpLocomotionState jump;
        internal RotationLocomotion rotation;

        protected override void Awake()
        {
            base.Awake();
            _definitions = GetComponent<ILocomotionDefinitions>() ?? throw new ComponentCantFindException(this.gameObject, typeof(ILocomotionDefinitions));
            walking = new(_definitions.Walking);
            jump = new(_definitions.Jump);
            quickBoostingHelper = new(_definitions.Walking, _definitions.QuickBoosting);
            quickBoosting = quickBoostingHelper.State;
            boosting = new(_definitions.Walking, _definitions.Boosting);
        }

        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            if (!blackboard.TryReadValue<IInput>(CharacterBlackboardFields.Input, out _input))
                throw new Exception();
            if (!blackboard.TryReadValue<Camera>(CharacterBlackboardFields.Character_Camera_Main, out var camera))
                throw new Exception();
            if (!blackboard.TryReadValue<Rigidbody>(CharacterBlackboardFields.Rigidbody, out var rbody))
                throw new Exception();
            blackboard.TryReadValue<World>(CharacterBlackboardFields.World, out var world);
            if (!blackboard.TryReadValue<IGroundDetector>(CharacterBlackboardFields.GroundDetector, out var groundDetector))
                throw new Exception();


            quickBoostingHelper.Input = _input;
            InitializeLocomotionCore(rbody, groundDetector, world);
            InitializeRotation(camera, rbody);
            InitializeMovementStatemachine();
            InitializeMainStatemachine(camera, rbody, world, groundDetector);

            _core.EvaluationModules = ArrayExtensions.Append_D(_core.EvaluationModules, _statemachine);
        }
        void InitializeLocomotionCore(Rigidbody rbody, IGroundDetector groundDetector, World world)
        {
            _core = new Core(rbody, groundDetector, new VerticalPostureEvaluator(_definitions.PostureEvaluationFramesQuantity));
            _core.World = world;
        }
        void InitializeRotation(Camera camera, Rigidbody rigidbody)
        {
            rotation = new(camera, rigidbody, _core);
            this.node.AddChild(rotation.Node);
        }
        void InitializeMovementStatemachine()
        {
            _context = new LocomotionStateContext(_core, _input);

            _movementStatemachine = new("movement", _context);
            _movementStatemachine.AddState(walking);
            _movementStatemachine.AddState(boosting);

            _movementStatemachine.AddTransitionFor(boosting, walking, () => _input.HorizontalVector == Vector3.zero);

        }
        void InitializeMainStatemachine(Camera camera, Rigidbody rigidbody, World world, IGroundDetector groundDetector)
        {

            _statemachine = new("main", _context);
            _statemachine.AddState(_movementStatemachine);
            _statemachine.AddState(jump);
            _statemachine.AddState(quickBoosting);

            _statemachine.AddTransitionFor(_movementStatemachine, quickBoosting, () => quickBoostingHelper.TriggerEvent);
            _statemachine.AddTransitionFor(_movementStatemachine, jump, () => groundDetector.Grounds.Count > 0 && _input.Jump);

            var j_m = new BlendingTransition<LocomotionStateContext>(jump, _movementStatemachine, () => _core.Context.VerticalPosture == VerticalPosture.Descending, null, 0, 0, 1);
            var j_qb = new BlendingTransition<LocomotionStateContext>(jump, quickBoosting, () => quickBoostingHelper.TriggerEvent, null, 0, 0, 0);
            _statemachine.AddTransitionFor(j_qb);
            _statemachine.AddTransitionFor(j_m);


            var qb_b = new SubStatemachineTransition<LocomotionStateContext>(quickBoosting, _movementStatemachine, boosting, null, null, 0, 0, 1);
            _statemachine.AddTransitionFor(qb_b);



            _core.EvaluationModules = ArrayExtensions.Append_D(_core.EvaluationModules, _statemachine);
        }
        void InitializeStatemachine(Rigidbody rigidbody, World world, IGroundDetector groundDetector)
        {
            _core = new Core(rigidbody, groundDetector, new VerticalPostureEvaluator(_definitions.PostureEvaluationFramesQuantity));
            _core.World = world;
            _context = new LocomotionStateContext(_core, _input);
            _statemachine = new("main", _context);

            quickBoostingHelper.Input = _input;

            _statemachine.AddState(walking);
            _statemachine.AddState(boosting);
            _statemachine.AddState(quickBoosting);
            _statemachine.AddState(jump);

            var w_t_qb = new BlendingTransition<LocomotionStateContext>(walking, quickBoosting, () => quickBoostingHelper.TriggerEvent, null, 0, 0, 0);
            var w_t_j = new BlendingTransition<LocomotionStateContext>(walking, jump, () => _input.Jump, null, 0, 0, 0);

            var qb_t_b = new BlendingTransition<LocomotionStateContext>(quickBoosting, boosting, null, null, 0, 0, 1);

            var b_t_w = new BlendingTransition<LocomotionStateContext>(boosting, walking, () => _input.HorizontalVector == Vector3.zero, null, 0, 0, 0);
            var b_t_j = new BlendingTransition<LocomotionStateContext>(boosting, jump, () => _input.Jump, null, 0, 0, 0);

            var j_t_w = new BlendingTransition<LocomotionStateContext>(
                jump,
                walking,
                () => _core.Context.VerticalPosture == VerticalPosture.Descending && _input.HorizontalVector == Vector3.zero,
                null,
                0,
                0,
                1);
            var j_t_b = new BlendingTransition<LocomotionStateContext>(
                jump,
                boosting,
                () => _core.Context.VerticalPosture == VerticalPosture.Descending && _input.HorizontalVector != Vector3.zero,
                null,
                0,
                0,
                1);
            var j_t_qb = new BlendingTransition<LocomotionStateContext>(jump, quickBoosting, () => quickBoostingHelper.TriggerEvent, null, 0, 0, 0);


            _statemachine.AddTransition(w_t_qb);
            _statemachine.AddTransition(w_t_j);

            _statemachine.AddTransition(b_t_j);
            _statemachine.AddTransition(b_t_w);

            _statemachine.AddTransition(qb_t_b);

            _statemachine.AddTransition(j_t_qb);
            _statemachine.AddTransition(j_t_b);
            _statemachine.AddTransition(j_t_w);


            //_core.EvaluationModules = _core.EvaluationModules.Append(_statemachine).ToArray();
            _core.EvaluationModules = ArrayExtensions.Append_D(_core.EvaluationModules, _statemachine);
        }
        private void LateUpdate()
        {
            rotation.OnUpdate();
        }
        public void FixedUpdate()
        {
            _context.Update();
            quickBoostingHelper.Update();
            _core.Update();

        }
    }
}
