using Tests.Behaviours.Arms.Weapons.Animations;
using UnityEngine;

namespace Tests.Behaviours.Arms.Animations
{
    internal class ArmAnimationDefinitions_MonoComponent
        : MonoBehaviour, IArmAnimationDefinitions
    {
        [SerializeField]
        ArmWeaponAnimationDefinitions _weapon;

        public IArmWeaponAnimationDefinitions Weapon => _weapon;
    }
}
