using Tests.Interaction;
using Tests.Interaction.Influences;

namespace Tests.Characters
{
    internal class CharacterAccessor_Debug : CharacterAccessor<Character_Debug>, ITeamMember, IDamageable, IKnockbackable
    {
        public TeamMask TeamMask { get => character.TeamMask; set => character.TeamMask = value; }

        public IHealth HP => character.HP;

        public IKnockback Knockback => character.Knockback;
    }
}
