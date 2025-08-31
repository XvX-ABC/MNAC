using System.Diagnostics.CodeAnalysis;

namespace Tests.Characters.Locomotion
{
    internal class WalkingState : MovementState
    {
        public WalkingState([NotNull] IWalkingDefinitions definitions, bool enabled = true) : base("walking", definitions, enabled)
        {

        }
    }
}
