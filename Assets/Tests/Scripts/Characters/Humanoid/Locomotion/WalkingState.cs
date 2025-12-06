using System.Diagnostics.CodeAnalysis;
using Tests.TPhysics.Environment;

namespace Tests.Characters.Humanoid.Locomotion
{
    internal class WalkingState : MovementState
    {
        public WalkingState([NotNull] IWalkingDefinitions definitions, IGroundDetector groundDetector, bool enabled = true) : base("walking", definitions, groundDetector, enabled)
        {

        }
    }
}
