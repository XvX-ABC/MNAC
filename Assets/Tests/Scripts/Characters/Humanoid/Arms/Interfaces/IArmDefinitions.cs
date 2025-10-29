using Tests.Characters.Arms.Weapons;
using Tests.Characters.Humanoid;
using UnityEngine;

namespace Tests.Characters.Arms
{
    public interface IArmDefinitions
    {
        public HumanPart Part { get; }
        public IArmedWeaponArmDefinitions Weapon { get; }
    }
}
