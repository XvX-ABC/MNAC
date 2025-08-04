using Tests.Assets;
using Tests.BodyBehaviour.Arm.Animations;
using UnityEngine;

namespace Assets.Tests.Scripts.BodyBehaviour.Arm.Animations
{
    public class ArmAnimationDefinitions : MonoBehaviour, IArmAnimationDefinitions
    {
        [SerializeField]
        ArmWeaponAnimationDefinitions _weapon;

        public IArmWeaponAnimationDefinitions Weapon => _weapon;
    }
}
