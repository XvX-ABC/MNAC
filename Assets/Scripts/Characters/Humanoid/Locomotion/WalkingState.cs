using System.Diagnostics.CodeAnalysis;
using Tests.TPhysics.Environment;

namespace Tests.Characters.Humanoid.Locomotion
{
    internal class WalkingState : MovementState
    {
        public WalkingState([NotNull] IWalkingDefinitions definitions, IMutativeDragDefinitions dragDefinitions, IGroundDetector groundDetector, bool enabled = true) : base("walking", definitions, dragDefinitions, groundDetector, enabled)
        {

        }
    }
}
