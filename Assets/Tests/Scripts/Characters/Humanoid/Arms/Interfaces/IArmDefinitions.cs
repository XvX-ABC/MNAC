using Tests.Behaviours.Arms.Animations;
using Tests.Characters.Humanoid;
using Tests.Characters.Humanoid.Arms.Weapons;
using UnityEngine;

namespace Tests.Characters.Humanoid.Arms
{
    public interface IArmDefinitions
    {
        IArmedWeaponArmDefinitions Weapon { get; }
        IArmAnimationDefinitions Animation { get; }
    }
}
