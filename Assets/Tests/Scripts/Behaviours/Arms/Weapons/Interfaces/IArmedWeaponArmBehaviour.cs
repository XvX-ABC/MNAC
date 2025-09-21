using System;
using Tests.Behaviours.Arms.Weapons;
using Tests.States;
using Tests.Weapons;

namespace Tests.Behaviours.Arms
{
    public interface IArmedWeaponArmBehaviour
    {
        public bool Activated { get; set; }
        public WeaponType Type { get; }
        public IWeapon Weapon { get; set; }
        public IArmedWeaponArmAnimationPlayablePart Animator { get; }
        public Func<bool> EntryFunc { get; }
        public Func<bool> ExitFunc { get; }

        public IWithCallbackPlayableState<object> State { get => null; }
    }
}
