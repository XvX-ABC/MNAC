using System;
using System.Collections.Generic;
using Tests.Weapons;

namespace Tests.Behaviours.Arms.Weapons
{
    internal interface IArmedWeaponArmBehavioursController<T> where T : IArmedWeaponArmBehaviour
    {
        Action<IWeapon, T> ActivatedAction { get; set; }
        Action<IWeapon, T> UnactivatedAction { get; set; }
        Func<bool> EntryFunc { get; }
        Func<bool> ExitFunc { get; }
        public IReadOnlyDictionary<string, T> Behaviours { get; }

        void ActivateBehaviourBy(IWeapon weapon);
        void UnactivateBehaviourBy(IWeapon weapon);
    }
}