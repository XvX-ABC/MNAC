using System;
using Tests.Behaviours.Arms.Animations;
using Tests.Characters.Humanoid.Arms.Weapons;
using UnityEngine;

namespace Tests.Characters.Humanoid.Arms
{
    [Obsolete]
    internal class ArmDefinitionsGetter_Mono : MonoBehaviour, IArmDefinitions
    {
        [SerializeField]
        ArmDefinitions_SO _definitions;

        public IArmedWeaponArmDefinitions Weapon => _definitions.Weapon;

        public IArmAnimationDefinitions Animation => _definitions.Animation;
    }
}
