using Tests.Utilities.MTrees;

namespace Tests.Characters
{
    public interface ICharacterComponent : ICharacterComponentDescriptions
    {
        public Blackboard Blackboard { get; set; }
        public ICharacterComponentNode Node { get; }
    }
    public interface ICharacterComponentNode : IMTContainerNode<ICharacterComponent>
    {
    }
}
