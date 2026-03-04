using System;
using UnityEngine;

namespace MNAC.Characters.Humanoid.Arms.Weapons
{
    public interface IArmedArmDefinitions : Behaviours.Arms.Weapons.IArmedArmDefinitions
    {
        IArmedArmBehaviour[] ArmedWeaponBehaviours { get; }
        [Obsolete]
        public IArmedArmBehaviour[] GetArmBehaviours(Transform parent);
    }
}
