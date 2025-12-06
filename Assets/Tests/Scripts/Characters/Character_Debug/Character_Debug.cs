using Tests.Animations;
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
        internal override void ComponentsDispose()
        {
            throw new System.NotImplementedException();
        }

        internal override InfluenceCore CreateInfluenceCore()
        {
            return default;
        }

        internal override CharacterBehavioursStatemachine CreateStatemachine()
        {
            throw new System.NotImplementedException();
        }

        internal override CharacterComponent[] GetComponents()
        {
            throw new System.NotImplementedException();
        }

        internal override AnimationPlayablePartBase GetMainAnimationPlayablePart()
        {
            throw new System.NotImplementedException();
        }

        internal override void InitializeComponents(Blackboard blackboard)
        {
            throw new System.NotImplementedException();
        }
    }
}
