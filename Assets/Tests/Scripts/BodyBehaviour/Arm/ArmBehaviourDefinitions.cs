using UnityEngine;

namespace Tests.BodyBehaviour.Arm
{
    public class ArmBehaviourDefinitions : MonoBehaviour, IArmBehaviourDefinitions
    {
        [SerializeField]
        ArmWeaponDefinitions _weapon;
        public IArmWeaponDefinitions Weapon => _weapon;

    }
}
