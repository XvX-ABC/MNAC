using Tests.Assets;
using Tests.Behaviours.Arms.Weapons.Animations;
using UnityEngine;

namespace Tests.Behaviours.Arms.Animations
{
    public class ArmAnimationDefinitions : MonoBehaviour, IArmAnimationDefinitions
    {
        [SerializeField]
        ArmWeaponAnimationDefinitions _weapon;

        public IArmWeaponAnimationDefinitions Weapon => _weapon;
    }
}
