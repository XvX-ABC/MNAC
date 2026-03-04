using System;
using MNAC.Behaviours.Arms.Animations;
using MNAC.Characters.Humanoid.Arms.Weapons;
using UnityEngine;

namespace MNAC.Characters.Humanoid.Arms
{
    [Obsolete]
    internal class ArmDefinitionsGetter_Mono : MonoBehaviour, IArmDefinitions
    {
        [SerializeField]
        ArmDefinitions_SO _definitions;

        public IArmedArmDefinitions Weapon => _definitions.Weapon;

        public IArmAnimationDefinitions Animation => _definitions.Animation;
    }
}
