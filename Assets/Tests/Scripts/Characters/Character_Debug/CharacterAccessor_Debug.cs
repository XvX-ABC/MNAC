using Tests.Interaction;

namespace Tests.Characters
{
    internal class CharacterAccessor_Debug : CharacterAccessor<Character_Debug>, ITeamMember, IDamageable
    {
        public TeamMask TeamMask { get => character.TeamMask; set => character.TeamMask = value; }

        public IHealth HP => character.HP;
    }
}
