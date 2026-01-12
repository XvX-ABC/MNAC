using UnityEngine;

namespace Tests.Behaviours.Arms.Animations
{
    public interface IArmAnimationDefinitions
    {
        public IArmWeaponAnimationDefinitions Weapon { get; }
    }
}
