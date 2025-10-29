using Tests.Characters.Arms.Weapons;
using Tests.Characters.Humanoid;
using UnityEngine;

namespace Tests.Characters.Arms
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
