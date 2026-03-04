using System;
using System.Diagnostics.CodeAnalysis;
using MNAC.Characters.Humanoid.Input;
using MNAC.States;
using MNAC.TPhysics.Environment;
using MNAC.TPhysics.Locomotion;
using UnityEngine;
using LCore = MNAC.TPhysics.Locomotion.LocomotionCore;
namespace MNAC.Characters.Humanoid.Locomotion
{
    internal class MovementState : LocomotionStateBase
    {

        protected HorizontalLocomotion locomotion;
        protected IGroundDetector groundDetector;
        LCore _lcore;
        bool _bindHorizontalVector;
        protected MutativeDrag mutativeDrag;
        protected MovementState(string name, IMovementDefinitions definitions, IMutativeDragDefinitions dragDefinitions, IGroundDetector groundDetector, bool enabled, bool bindHorizontalVector = true) : this(
            name,
            definitions.MaxSpeed,
            definitions.AcceleratedSpeed,
            0,
            dragDefinitions.TransitionDuration,
            dragDefinitions.Range,
            groundDetector,
            enabled,
            bindHorizontalVector)
        {
        }
        protected MovementState(
            string name,
            float maxSpeed,
            float acceleratedSpeed,
            float duration,
            float dragTransitionDuration,
           Vector2 dragRange,
            IGroundDetector groundDetector,
            bool enabled,
            bool bindHorizontalVector = true) : base(name == null ? "movement" : $"movement_{name}", duration, enabled)
        {
            this.groundDetector = groundDetector ?? throw new ArgumentNullException(nameof(groundDetector));
            locomotion = new HorizontalLocomotion(maxSpeed, acceleratedSpeed);
            _bindHorizontalVector = bindHorizontalVector;
            mutativeDrag = new(dragTransitionDuration, dragRange);
        }
        public override object Context
        {
            get => base.Context;
            set
            {
                if (base.Context is LocomotionStateContext ctx_0)
                {
                    ctx_0.Core.RemoveModule(mutativeDrag);
                }
                if (value is LocomotionStateContext ctx_1)
                {
                    _lcore = ctx_1.Core;
                    _lcore.AddModule(mutativeDrag);
                }

                if (_bindHorizontalVector)
                {
                    if (base.Context != null && base.Context != value)
                    {
                        (base.Context as LocomotionStateContext).InputBoundAction -= UpdateHorizontalVector;
                    }
                    if (value != null)
                        (value as LocomotionStateContext).InputBoundAction += UpdateHorizontalVector;
                }
                base.Context = value;
            }
        }
        protected override ILocomotionModule module => locomotion;
        public override void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionBegin(currentTransition);
            //context.Core.EnableModule(module);
        }
        public override void OnUpdate()
        {
            base.OnUpdate();
            var input = context.input;
            var direction = input.HorizontalVector;
            if (direction == Vector3.zero && groundDetector.Grounds.Count > 0)
                _lcore.EnableModule(mutativeDrag);
            else
                _lcore.DisableModule(mutativeDrag);
        }
        public override void OnExit()
        {
            _lcore.DisableModule(mutativeDrag);
            base.OnExit();
        }
        void UpdateHorizontalVector(IHumanoidInput input)
        {
            if (locomotion.Enabled)
            {
                //locomotion.HorizontalVector = context.Input_Obsolete.HorizontalVector;
                locomotion.DirectionVector = input.HorizontalVector;
            }
        }

    }
}
