using System;
using Tests.Extensions;
using Tests.Input;
using Tests.States;
using Tests.TPhysics;
using Tests.TPhysics.Environment;
using Tests.TPhysics.Locomotion;
using UnityEngine;
using LCore = Tests.TPhysics.Locomotion.LocomotionCore;
using LContext = Tests.TPhysics.Locomotion.Context;
using Tests.Characters.Locomotion.Animations;
namespace Tests.Characters.Locomotion
{

    internal class LocomotionCore : ComponentBase_MonoComponent
    {
        internal ILocomotionDefinitions definitions;

        IInput _input;

        LCore _core;
        internal LocomotionStatemachine movementStatemachine;
        internal LocomotionStatemachine statemachine;
        internal LocomotionStateContext context;

        internal QuickBoostingHelper quickBoostingHelper;

        internal BoostingState boosting;
        internal QuickBoostingState quickBoosting;
        internal WalkingState walking;
        internal JumpLocomotionState jump;
        internal RotationLocomotion rotation;


        internal LocomotionAnimator animator;

        internal LCore core => _core;
        internal LContext locomotionContext => _core.Context;

        protected override void Awake()
        {
            base.Awake();
            definitions = GetComponent<ILocomotionDefinitions>() ?? throw new ComponentCantFindException(this.gameObject, typeof(ILocomotionDefinitions));
            walking = new(definitions.Walking);
            jump = new(definitions.Jump);
            quickBoostingHelper = new(definitions.Walking, definitions.QuickBoosting);
            quickBoosting = quickBoostingHelper.State;
            boosting = new(definitions.Walking, definitions.Boosting);
            animator = new(definitions.Animation, this);
        }
        private void OnEnable()
        {
            if (statemachine != null)
            {
                statemachine.Enabled = true;
                statemachine.OnEnter();
            }
        }
        private void OnDisable()
        {
            statemachine.OnExit();
            statemachine.Enabled = false;
        }
        void InitializeRigidbody(Rigidbody rbody)
        {
            rbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        }
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            if (!blackboard.TryReadValue<IInput>(CharacterBlackboardFields.Character_Input_Main, out _input))
                throw new Exception();
            if (!blackboard.TryReadValue<Camera>(CharacterBlackboardFields.Character_Camera_Main, out var camera))
                throw new Exception();
            if (!blackboard.TryReadValue<Rigidbody>(CharacterBlackboardFields.Rigidbody, out var rbody))
                throw new Exception();
            else
                InitializeRigidbody(rbody);

            if (!blackboard.TryReadValue<World>(CharacterBlackboardFields.World, out var world))
                throw new Exception();
            if (!blackboard.TryReadValue<IGroundDetector>(CharacterBlackboardFields.GroundDetector, out var groundDetector))
                throw new Exception();


            quickBoostingHelper.Input = _input;
            InitializeLocomotionCore(rbody, groundDetector, world);
            InitializeRotation(camera, rbody);
            InitializeMovementStatemachine();
            InitializeMainStatemachine(camera, rbody, world, groundDetector);

            _core.EvaluationModules = ArrayExtensions.Append_D(_core.EvaluationModules, statemachine);

            blackboard.TryRegisterField(CharacterBlackboardFields.Character_Locomotion_Core, this);

            this.node.AddChild(animator.node);
        }

        void InitializeLocomotionCore(Rigidbody rbody, IGroundDetector groundDetector, World world)
        {
            _core = new LCore(rbody, groundDetector, new VerticalPostureEvaluator(definitions.PostureEvaluationFramesQuantity));
            _core.World = world;
        }
        void InitializeRotation(Camera camera, Rigidbody rigidbody)
        {
            rotation = new(camera, rigidbody, _core);
            this.node.AddChild(rotation.Node);
        }
        void InitializeMovementStatemachine()
        {
            context = new LocomotionStateContext(_core, _input);

            movementStatemachine = new("movement", context);
            movementStatemachine.AddState(walking);
            movementStatemachine.AddState(boosting);

            movementStatemachine.AddTransitionFor(boosting, walking, () => _input.HorizontalVector == Vector3.zero);

        }
        void InitializeMainStatemachine(Camera camera, Rigidbody rigidbody, World world, IGroundDetector groundDetector)
        {

            statemachine = new("main", context);
            statemachine.AddState(movementStatemachine);
            statemachine.AddState(jump);
            statemachine.AddState(quickBoosting);

            statemachine.AddTransitionFor(movementStatemachine, quickBoosting, () => quickBoostingHelper.TriggerEvent);
            statemachine.AddTransitionFor(movementStatemachine, jump, () => groundDetector.Grounds.Count > 0 && _input.Jump);

            var j_m = new BlendingTransition<object>(jump, movementStatemachine, () => _core.Context.VerticalPosture == VerticalPosture.Descending, null, 0, 0, 1);
            var j_qb = new BlendingTransition<object>(jump, quickBoosting, () => quickBoostingHelper.TriggerEvent, null, 0, 0);
            statemachine.AddTransitionFor(j_qb);
            statemachine.AddTransitionFor(j_m);


            var qb_b = new SubStatemachineTransition<object>(quickBoosting, movementStatemachine, boosting, null, null, 0, 0, 1);
            statemachine.AddTransitionFor(qb_b);



            _core.EvaluationModules = ArrayExtensions.Append_D(_core.EvaluationModules, statemachine);


        }
        private void LateUpdate()
        {
            rotation.OnUpdate();
        }
        public void FixedUpdate()
        {
            context.Update();
            quickBoostingHelper.Update();
            _core.Update();
            var pos = _core.Context.CurrentPosition;
            Debug.DrawLine(pos, pos + _core.Context.CurrentVelocity, Color.magenta);
            //animator.Update();
            //Debug.Log(statemachine);

        }
    }
}
