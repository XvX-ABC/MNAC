using MNAC.Behaviours.Arms.Animations;
using MNAC.Characters.Humanoid;
using MNAC.Characters.Humanoid.Arms.Weapons;
using UnityEngine;

namespace MNAC.Characters.Humanoid.Arms
{
    public interface IArmDefinitions
    {
        IArmedArmDefinitions Weapon { get; }
        IArmAnimationDefinitions Animation { get; }
    }
}
