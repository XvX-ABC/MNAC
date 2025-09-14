using System.Diagnostics.CodeAnalysis;
using Tests.Input;
using Tests.States;
using Tests.TPhysics.Locomotion;
using UnityEngine;
using Utilities.Timeline;

namespace Tests.Characters.Locomotion
{
    internal class QuickBoostingHelper
    {
        QuickBoostingState _state;
        ITimeline _cdTimeline;
        IInput _input;
        public QuickBoostingHelper([NotNull] IMovementDefinitions movementDefinitions, [NotNull] IQuickBoostingDefinitions definitions, bool enabled = true)
        {
            _state = new QuickBoostingState(movementDefinitions, definitions, enabled);
            _cdTimeline = new Timeline_V1(definitions.ColdDownTime);

            _state.ExitAction += () => _cdTimeline.Restart();
            _cdTimeline.SetNormalizedTime(1);
        }

        public QuickBoostingState State { get => _state; set => _state = value; }
        public bool IsColdDowned
        {
            get => _cdTimeline.NormalizedTime >= 1;
        }
        public IInput Input { get => _input; set => _input = value; }

        public bool TriggerEvent
        {
            get => _input == null ? false : _input.HorizontalVector != Vector3.zero && _input.QuickBoost && IsColdDowned;
        }
        public void Update()
        {
            _cdTimeline.OnUpdate(Time.deltaTime);
        }
    }
    internal class QuickBoostingState : MovementState
    {
        public QuickBoostingState(
            [NotNull] IMovementDefinitions movementDefinitions,
            [NotNull] IQuickBoostingDefinitions definitions,
            bool enabled = true) : base(
                "quick_boosting",
                movementDefinitions.MaxSpeed * (definitions.MaxSpeedPower < 1 ? 1 : definitions.MaxSpeedPower),
                movementDefinitions.AcceleratedSpeed * (definitions.AcceleratedSpeedPower < 1 ? 1 : definitions.AcceleratedSpeedPower),
                definitions.Duration,
                enabled,
                false)
        {
        }
        public override void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<object> currentTransition)
        {
            base.FromPreviousStateTransitionBegin(currentTransition);
            locomotion.HorizontalVector = context.Input.HorizontalVector;
        }
        public override void OnExit()
        {
            locomotion.HorizontalVector = Vector3.zero;
            base.OnExit();
        }
    }
}
