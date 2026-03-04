using MNAC.Behaviours.Arms.Animations;
using MNAC.Characters.Humanoid.Arms.Weapons;
using UnityEngine;

namespace MNAC.Characters.Humanoid.Arms
{
    [CreateAssetMenu(fileName = "ArmDefinitions", menuName = "Tests/Definitions/Characters/Humanoid/Arms/ArmDefinitions")]
    internal class ArmDefinitions_SO : ScriptableObject, IArmDefinitions
    {
        [SerializeField]
        ArmDefinitions _definitions;


        public IArmedArmDefinitions Weapon => _definitions.Weapon;

        public IArmAnimationDefinitions Animation => _definitions.Animation;
    }
}
