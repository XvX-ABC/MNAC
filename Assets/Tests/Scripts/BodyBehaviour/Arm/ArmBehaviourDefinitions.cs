using UnityEngine;

namespace Tests.Behaviours.Arm
{
    public class ArmBehaviourDefinitions : MonoBehaviour, IArmBehaviourDefinitions
    {
        [SerializeField]
        ArmWeaponDefinitions _weapon;
        public IArmWeaponDefinitions Weapon => _weapon;

    }
}
