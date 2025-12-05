using Tests.Characters.Humanoid;
using Tests.Characters.Interaction;
using Tests.Interaction;
using Tests.Interaction.Influence;
using Tests.Utilities.Blackboards;

namespace Tests.Characters
{
    [Interactable]
    public class Character_Debug : CharacterBase
    {
        internal override InfluenceCore CreateInfluenceCore()
        {
            return default;
        }

        internal override CharacterBehavioursStatemachine CreateStatemachine(NormalState normalState, InfluenceCore influenceCore)
        {
            return default;
        }

        internal override NormalState InitializeController(Blackboard blackboard)
        {
            return default;
        }
    }
}
