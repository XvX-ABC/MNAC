using System.Diagnostics.CodeAnalysis;
using MNAC.TPhysics.Environment;

namespace MNAC.Characters.Humanoid.Locomotion
{
    internal class BoostingState : MovementState
    {
        public BoostingState([NotNull] IMovementDefinitions movementDefinitions, IBoostingDefinitions definitions, IMutativeDragDefinitions dragDefinitions, IGroundDetector groundDetector, bool enabled = true) : base(
            "boosting",
            movementDefinitions.MaxSpeed * (definitions.MaxSpeedPower < 1 ? 1 : definitions.MaxSpeedPower),
            movementDefinitions.AcceleratedSpeed * (definitions.AcceleratedSpeedPower < 1 ? 1 : definitions.AcceleratedSpeedPower),
            0,
            dragDefinitions.TransitionDuration,
            dragDefinitions.Range,
            groundDetector,
            enabled)
        {

        }
    }
}
