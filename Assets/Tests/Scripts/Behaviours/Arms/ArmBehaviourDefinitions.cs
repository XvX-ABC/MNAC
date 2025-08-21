using Tests.Behaviours.Arms.Weapons;
using UnityEngine;

namespace Tests.Behaviours.Arms
{
    public class ArmBehaviourDefinitions : MonoBehaviour, IArmDefinitions
    {
        [SerializeField]
        ArmWeaponDefinitions _weapon;
        [SerializeField]
        HumanPartDof _part;
        public HumanPartDof Part { get => _part; }
        public IArmWeaponDefinitions Weapon => _weapon;

    }
}
