using System;
using MNAC.Behaviours.Arms.Weapons;
using MNAC.States;
using MNAC.Weapons;
using MNAC.Weapons;
using UnityEngine;
using WeaponType = MNAC.Weapons.WeaponType;

namespace MNAC.Behaviours.Arms
{
    public interface IArmedArmBehaviour
    {
        public bool Activated { get; set; }
        public WeaponType Type { get; }
        public IWeapon Weapon { get; set; }
        public IArmedArmAnimationPlayablePart Animator { get; }
        public Func<bool> ActivationTrigger { get; }
        public Func<bool> UnactivationTrigger { get; }

        public IWithCallbackPlayableState<object> State { get => null; }
        public void BehaviourOnUpdate();
        public void BehaviourOnLateUpdate();
        public void BehaviourOnFixedUpdate();
    }
}
