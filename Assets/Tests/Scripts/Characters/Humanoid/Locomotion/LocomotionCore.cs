using System;
using Tests.AI;
using Tests.Behaviours.Input;
using Tests.Characters.Humanoid.Input;
using Tests.Characters.Humanoid.Locomotion.Animations;
using Tests.Extensions;
using Tests.States;
using Tests.TPhysics;
using Tests.TPhysics.Environment;
using Tests.TPhysics.Locomotion;
using Tests.Utilities.Blackboards;
using Tests.Utilities.Composable;
using Unity.VisualScripting;
using UnityEngine;
using LContext = Tests.TPhysics.Locomotion.Context;
using LCore = Tests.TPhysics.Locomotion.LocomotionCore;
namespace Tests.Characters.Humanoid.Locomotion
{

    internal class LocomotionCore : HumanoidComponent
    {
        internal ILocomotionDefinitions definitions;

        IHumanoidInput _input;
        LCore _core;
        internal LocomotionStatemachine movementStatemachine;
        internal LocomotionStatemachine statemachine;
        internal LocomotionStateContext context;

        internal QuickBoostingHelper quickBoostingHelper;


        internal BoostingState boosting;
        internal QuickBoostingState quickBoosting;
        internal WalkingState walking;
        internal JumpLocomotionState jump;
        RotationLocomotionBase _rotationModule;
        MutativeDragController _mutativeDragControl;


        internal LocomotionAnimator animator;

        internal LCore internalCore => _core;
        internal LContext locomotionContext => _core.Context;
        internal RotationLocomotionBase rotationModule
        {
            get => _rotationModule;
            set
            {
                if (_rotationModule != null)
                    this.Node.RemoveChild(_rotationModule.Node);
                if (value != null)
                {
                    this.Node.AddChild(value.Node);
                }
                else
                    throw new NullReferenceException(nameof(_rotationModule));
                _rotationModule = value;
            }
        }

        internal MutativeDragController MutativeDragControl
        {
            get => _mutativeDragControl;
        }

        protected override void Awake()
        {
            base.Awake();
            definitions = GetComponent<ILocomotionDefinitions>() ?? throw new ComponentCantFindException(gameObject, typeof(ILocomotionDefinitions));

        }
        private void OnEnable()
        {
            if (statemachine != null)
                statemachine.Enabled = true;
        }
        private void OnDisable()
        {
            if (statemachine != null)
                statemachine.Enabled = false;
        }
        void InitializeRigidbody(Rigidbody rbody)
        {
            rbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        }
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            owner.locomotionCore = this;

            if (!blackboard.TryReadValue<Camera>(CharacterBlackboardFields.Player_Camera_Main, out var camera))
                throw new Exception();
            if (!blackboard.TryReadValue<Rigidbody>(CharacterBlackboardFields.Rigidbody, out var rbody))
                throw new Exception();
            else
                InitializeRigidbody(rbody);

            if (!blackboard.TryReadValue<World>(CharacterBlackboardFields.World, out var world))
                throw new Exception();
            if (!blackboard.TryReadValue<IGroundDetector>(CharacterBlackboardFields.GroundDetector, out var groundDetector))
                throw new Exception();

            blackboard.TryReadValueOrThrowException(CharacterBlackboardFields.Character_Input_Main, out _input);

            walking = new(definitions.Walking, groundDetector);
            jump = new(definitions.Jump.Height);
            quickBoostingHelper = new(definitions.Walking, definitions.QuickBoosting);
            quickBoosting = quickBoostingHelper.State;
            boosting = new(definitions.Walking, definitions.Boosting, groundDetector);
            animator = new(definitions.Animation, this);



            quickBoostingHelper.Input = _input;
            InitializeLocomotionCore(rbody, groundDetector, world);
            InitializeRotation(camera, rbody, _input.BaseInput);
            InitializeMovementStatemachine();
            InitializeMainStatemachine(rbody, world, groundDetector);
            InitializeMutativeDragControl(groundDetector, _input.BaseInput);

            _core.EvaluationModules = ArrayExtensions.Append(_core.EvaluationModules, statemachine);

            blackboard.TryRegisterField(CharacterBlackboardFields.Character_Locomotion_Core, this);
            Node.AddChild(animator.Node);
        }

        void InitializeLocomotionCore(Rigidbody rbody, IGroundDetector groundDetector, World world)
        {
            _core = new LCore(world, rbody, groundDetector, new VerticalPostureEvaluator(definitions.PostureEvaluationFramesAmount));
            _core.World = world;
        }
        void InitializeRotation(Camera camera, Rigidbody rigidbody, IBaseInput input)
        {
            rotationModule = new RotationByPlayerLocomotion(camera, rigidbody, _core, input);
        }
        void InitializeMutativeDragControl(IGroundDetector groundDetector, IBaseInput input)
        {
            _mutativeDragControl = new(
                definitions.MutativeDrag.TransitionDuration,
                definitions.MutativeDrag.Range,
                groundDetector,
                input,
                _core,
                statemachine,
                movementStatemachine);
        }
        void InitializeMovementStatemachine()
        {
            context = new LocomotionStateContext(_core, _input);

            movementStatemachine = new("movement", context);
            movementStatemachine.AddState(walking);
            movementStatemachine.AddState(boosting);

            movementStatemachine.AddTransitionFor(boosting, walking, () => _input.HorizontalVector == Vector3.zero);

        }
        void InitializeMainStatemachine(Rigidbody rigidbody, World world, IGroundDetector groundDetector)
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


            var qb_b = new SubStatemachineTransition<object>(quickBoosting, movementStatemachine, boosting, null, null, 0, 0, 1, InterruptionSource.None);
            statemachine.AddTransitionFor(qb_b);


        }
        public void AddModule(ILocomotionModule module)
        {
            _core.AddModule(module);
        }
        public void RemoveModule(ILocomotionModule module)
        {
            _core.RemoveModule(module);
        }
        private void LateUpdate()
        {

        }
        public void FixedUpdate()
        {
            context.Update();
            quickBoostingHelper.Update();
            _core.Update();
            var pos = _core.Context.CurrentPosition;
            _rotationModule.OnUpdate();
            _mutativeDragControl?.OnUpdate();
            Debug.DrawLine(pos, pos + _core.Context.CurrentVelocity, Color.magenta);
            //animator.Update();
            //Debug.Log(statemachine);

        }

        private void OnDrawGizmos()
        {
            var pos = transform.position;
            var fpos = pos + transform.forward * 500;

            Gizmos.color = Color.yellow;
            //Gizmos.DrawLine(pos, fpos);
        }

        internal void EnableModule(ILocomotionModule module)
        {
            _core.EnableModule(module);
        }

        internal void DisableModule(ILocomotionModule module)
        {
            _core.DisableModule(module);
        }
    }
}
