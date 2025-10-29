using Tests.Characters.Humanoid;
using Tests.Characters.Humanoid.Arms.Weapons;
using UnityEngine;

namespace Tests.Characters.Humanoid.Arms
{
    internal class ArmDefinitions : MonoBehaviour, IArmDefinitions
    {
        [SerializeField]
        ArmWeaponDefinitions _weapon;
        [SerializeField]
        HumanPart _part;
        public HumanPart Part { get => _part; }
        public IArmedWeaponArmDefinitions Weapon => _weapon;

    }
}
