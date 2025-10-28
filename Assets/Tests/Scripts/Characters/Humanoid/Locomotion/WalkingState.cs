using System.Diagnostics.CodeAnalysis;

namespace Tests.Characters.Humanoid.Locomotion
{
    internal class WalkingState : MovementState
    {
        public WalkingState([NotNull] IWalkingDefinitions definitions, bool enabled = true) : base("walking", definitions, enabled)
        {

        }
    }
}
