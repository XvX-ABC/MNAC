using Tests.Behaviours.Arms.Animations;
using Tests.Characters.Humanoid.Arms.Weapons;
using UnityEngine;

namespace Tests.Characters.Humanoid.Arms
{
    internal class ArmDefinitionsGetter_MonoComponent : MonoBehaviour, IArmDefinitions
    {
        [SerializeField]
        ArmDefinitions_SO _definitions;
        public HumanBodyPart Part => _definitions.Part;

        public IArmedWeaponArmDefinitions Weapon => _definitions.Weapon;

        public IArmAnimationDefinitions Animation => _definitions.Animation;
    }
}
