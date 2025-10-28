using System.Diagnostics.CodeAnalysis;

namespace Tests.Characters.Humanoid.Locomotion
{
    internal class BoostingState : MovementState
    {
        public BoostingState([NotNull] IMovementDefinitions movementDefinitions, [NotNull] IBoostingDefinitions definitions, bool enabled = true) : base("boosting", movementDefinitions.MaxSpeed * (definitions.MaxSpeedPower < 1 ? 1 : definitions.MaxSpeedPower), movementDefinitions.AcceleratedSpeed * (definitions.AcceleratedSpeedPower < 1 ? 1 : definitions.AcceleratedSpeedPower), 0, enabled)
        {

        }
    }
}
