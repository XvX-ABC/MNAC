using System;
using System.Diagnostics.CodeAnalysis;
using Tests.Characters.Humanoid.Interaction.Input;
using Tests.Input;
using Tests.States;
using Tests.TPhysics.Environment;
using Tests.TPhysics.Locomotion;
using UnityEngine;
using LCore = Tests.TPhysics.Locomotion.LocomotionCore;
namespace Tests.Characters.Humanoid.Locomotion
{
    internal class MovementState : LocomotionStateBase
    {

        protected HorizontalLocomotion locomotion;
        protected MutativeDrag mutativeDrag;
        protected IGroundDetector groundDetector;
        LCore _lcore;
        bool _bindHorizontalVector;
        protected MovementState(string name, [NotNull] IMovementDefinitions definitions, IGroundDetector groundDetector, bool enabled, bool bindHorizontalVector = true) : this(
            name,
            definitions.MaxSpeed,
            definitions.AcceleratedSpeed,
            0,
            definitions.DragTransitionalDuration,
            definitions.DragTransitionalRange,
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
            float dragTransitionalDuration,
            Vector2 dragTransitionalRange,
            IGroundDetector groundDetector,
            bool enabled,
            bool bindHorizontalVector = true) : base(name == null ? "movement" : $"movement_{name}", duration, enabled)
        {
            this.groundDetector = groundDetector ?? throw new ArgumentNullException(nameof(groundDetector));
            locomotion = new HorizontalLocomotion(maxSpeed, acceleratedSpeed);
            mutativeDrag = new(dragTransitionalDuration, dragTransitionalRange);
            _bindHorizontalVector = bindHorizontalVector;
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
            context.Core.EnableModule(module);
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
        void UpdateHorizontalVector(IHumanInput input)
        {
            if (locomotion.Enabled)
            {
                //locomotion.HorizontalVector = context.Input_Obsolete.HorizontalVector;
                locomotion.DirectionVector = input.HorizontalVector;
            }
        }

    }
}
