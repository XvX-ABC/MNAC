using System.Diagnostics.CodeAnalysis;
using MNAC.TPhysics.Environment;

namespace MNAC.Characters.Humanoid.Locomotion
{
    internal class WalkingState : MovementState
    {
        public WalkingState([NotNull] IWalkingDefinitions definitions, IMutativeDragDefinitions dragDefinitions, IGroundDetector groundDetector, bool enabled = true) : base("walking", definitions, dragDefinitions, groundDetector, enabled)
        {

        }
    }
}
