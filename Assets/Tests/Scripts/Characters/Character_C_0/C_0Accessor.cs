using Tests.Interaction;
using UnityEngine;

namespace Tests.Characters.C_0
{
    internal class C_0Accessor : CharacterAccessor<C_0>, IDamageable, ITeamMember, ICompositeItems
    {
        public IHealth HP => character.HP;

        public TeamMask TeamMask { get => character.TeamMask; set => character.TeamMask = value; }

        public GameObject GetItem(uint key)
        {
            return character.GetItem(key);
        }
    }
}
