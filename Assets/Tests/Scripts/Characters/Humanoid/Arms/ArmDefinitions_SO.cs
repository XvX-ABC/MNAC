using Tests.Behaviours.Arms.Animations;
using Tests.Characters.Humanoid.Arms.Weapons;
using UnityEngine;

namespace Tests.Characters.Humanoid.Arms
{
    [CreateAssetMenu(fileName = "ArmDefinitions", menuName = "Tests/Definitions/Characters/Humanoid/Arms/ArmDefinitions")]
    internal class ArmDefinitions_SO : ScriptableObject, IArmDefinitions
    {
        [SerializeField]
        ArmDefinitions _definitions;

        public HumanBodyPart Part => _definitions.Part;

        public IArmedWeaponArmDefinitions Weapon => _definitions.Weapon;

        public IArmAnimationDefinitions Animation => _definitions.Animation;
    }
}
