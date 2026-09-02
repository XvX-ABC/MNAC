using UnityEngine;

namespace MNAC.Behaviours.Arms.Animations
{
    public interface IArmAnimationDefinitions
    {
        public IArmWeaponAnimationDefinitions Weapon { get; }
    }
}
