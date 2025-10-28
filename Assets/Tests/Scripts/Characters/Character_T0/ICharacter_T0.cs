using Tests.Interaction;
using Tests.Interaction.Influence;

namespace Tests.Characters.Interaction
{
    public interface ICharacter_T0 : ICharacter, IDamageable
    {
        public IStun Stun { get; }
        public InfluenceCore InfluenceCore { get; }
    }
}
