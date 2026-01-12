using System;
using Tests.Behaviours.Arms.Weapons;
using Tests.States;
using Tests.Weapons;
using Tests.Weapons_New;
using UnityEngine;
using WeaponType = Tests.Weapons_New.WeaponType;

namespace Tests.Behaviours.Arms
{
    public interface IArmedWeaponArmBehaviour
    {
        public bool Activated { get; set; }
        public WeaponType Type { get; }
        public IWeapon Weapon { get; set; }
        public IArmedWeaponArmAnimationPlayablePart Animator { get; }
        public Func<bool> ActivationTrigger { get; }
        public Func<bool> UnactivationTrigger { get; }

        public IWithCallbackPlayableState<object> State { get => null; }
        public void BehaviourOnUpdate();
        public void BehaviourOnLateUpdate();
        public void BehaviourOnFixedUpdate();
    }
}
