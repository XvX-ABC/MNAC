using Tests.Characters.Arms.Weapons;
using UnityEngine;

namespace Tests.Characters.Arms
{
    internal class ArmDefinitions : MonoBehaviour, IArmDefinitions
    {
        [SerializeField]
        ArmWeaponDefinitions _weapon;
        [SerializeField]
        HumanPartDof _part;
        public HumanPartDof Part { get => _part; }
        public IArmWeaponDefinitions Weapon => _weapon;

    }
}
