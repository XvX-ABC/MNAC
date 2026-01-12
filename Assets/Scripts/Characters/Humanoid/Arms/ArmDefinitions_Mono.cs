using Tests.Behaviours.Arms.Animations;
using Tests.Characters.Humanoid;
using Tests.Characters.Humanoid.Arms.Weapons;
using UnityEngine;

namespace Tests.Characters.Humanoid.Arms
{
    internal class ArmDefinitions_Mono : MonoBehaviour, IArmDefinitions
    {
        [SerializeField]
        ArmWeaponDefinitions _weapon;
        [SerializeField]
        ArmAnimationDefinitions _animation;
        [SerializeField]
        HumanBodyPart _part;
        public HumanBodyPart Part { get => _part; }
        public IArmedWeaponArmDefinitions Weapon => _weapon;

        public IArmAnimationDefinitions Animation => _animation;
    }
}
