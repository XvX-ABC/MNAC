using Tests.Characters.Arms.Weapons;
using UnityEngine;

namespace Tests.Characters.Arms
{
    public interface IArmDefinitions
    {
        public HumanPartDof Part { get; }
        public IArmWeaponDefinitions Weapon { get; }
    }
}
