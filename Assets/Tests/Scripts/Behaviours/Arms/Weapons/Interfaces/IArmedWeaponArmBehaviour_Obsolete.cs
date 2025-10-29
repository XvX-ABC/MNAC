using System;
using Tests.Behaviours.Arms.Weapons;
using Tests.Characters.Humanoid.Arms;
using Tests.States;
using Tests.Weapons;

namespace Tests.Behaviours.Arms
{
    [Obsolete]
    internal interface IArmedWeaponArmBehaviour_Obsolete : IArmBehaviour, IArmedWeaponArmBehaviour
    {
        public WeaponType Type { get; }
        public IWeapon Weapon { get; set; }
        public IPlayableState<object> StateNode { get; }
        public IArmedWeaponArmAnimationPlayablePart Animator { get; }

        public Func<bool> EntryFunc { get; set; }
        public Func<bool> ExitFunc { get; set; }

    }
}
