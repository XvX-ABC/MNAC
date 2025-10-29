using Tests.Characters.Humanoid;
using Tests.Characters.Humanoid.Arms.Weapons;
using UnityEngine;

namespace Tests.Characters.Humanoid.Arms
{
    public interface IArmDefinitions
    {
        public HumanPart Part { get; }
        public IArmedWeaponArmDefinitions Weapon { get; }
    }
}
