using Assets.Scripts.Utilities.Timeline;
using System;
using Tests.Behaviours.Arm.Weapons;
using Tests.States;
using Tests.Weapons;
using UnityEngine;
using UnityEngine.Playables;

namespace Tests.Behaviours.Arm
{

    public interface IArmedWeaponArmAnimator : IDynamicPlayablePart
    {
    }
    internal interface IArmedWeaponArmBehaviour : IArmBehaviour
    {
        public WeaponType Type { get; }
        public IWeapon Weapon { get; set; }
        public IPlayableState<object> State { get; }
        [Obsolete]
        public byte StatusNum { get; }
        [Obsolete]
        public bool IsActivated { get; }
        public IArmedWeaponArmAnimator Animator { get; }

    }
}
