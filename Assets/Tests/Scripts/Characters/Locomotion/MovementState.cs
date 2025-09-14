using System.Diagnostics.CodeAnalysis;
using Tests.Input;
using Tests.States;
using Tests.TPhysics.Locomotion;
using UnityEngine;

namespace Tests.Characters.Locomotion
{
    internal class MovementState : LocomotionStateBase
    {
        protected HorizontalLocomotion locomotion;
        bool _bindHorizontalVector;
        protected MovementState(string name, [NotNull] IMovementDefinitions definitions, bool enabled, bool bindHorizontalVector = true) : this(name, definitions.MaxSpeed, definitions.AcceleratedSpeed, 0, enabled, bindHorizontalVector)
        {
        }
        protected MovementState(string name, float maxSpeed, float acceleratedSpeed, float duration, bool enabled, bool bindHorizontalVector = true) : base(name == null ? "movement" : $"movement_{name}", duration, enabled)
        {
            locomotion = new HorizontalLocomotion(maxSpeed, acceleratedSpeed);
            _bindHorizontalVector = bindHorizontalVector;
        }
        public override object Context
        {
            get => base.Context;
            set
            {
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
        void UpdateHorizontalVector(IInput input)
        {
            if (locomotion.Enabled)
                locomotion.HorizontalVector = context.Input.HorizontalVector;
        }

    }
}
