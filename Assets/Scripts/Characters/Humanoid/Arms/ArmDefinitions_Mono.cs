using MNAC.Behaviours.Arms.Animations;
using MNAC.Characters.Humanoid;
using MNAC.Characters.Humanoid.Arms.Weapons;
using UnityEngine;

namespace MNAC.Characters.Humanoid.Arms
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
        public IArmedArmDefinitions Weapon => _weapon;

        public IArmAnimationDefinitions Animation => _animation;
    }
}
