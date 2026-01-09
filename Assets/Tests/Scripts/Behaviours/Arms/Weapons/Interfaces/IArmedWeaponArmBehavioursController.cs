using System;
using System.Collections.Generic;
using Tests.Weapons;
using Tests.Weapons_New;

namespace Tests.Behaviours.Arms.Weapons
{
    internal interface IArmedWeaponArmBehavioursController<T> where T : IArmedWeaponArmBehaviour
    {
        Action<IWeapon, T> ActivatedAction { get; set; }
        Action<IWeapon, T> UnactivatedAction { get; set; }
        Func<bool> ActivationTrigger { get; }
        Func<bool> UnactivationTrigger { get; }
        public IReadOnlyDictionary<string, T> Behaviours { get; }

        void ActivateBehaviourBy(IWeapon weapon);
        void UnactivateBehaviourBy(IWeapon weapon);
    }
}