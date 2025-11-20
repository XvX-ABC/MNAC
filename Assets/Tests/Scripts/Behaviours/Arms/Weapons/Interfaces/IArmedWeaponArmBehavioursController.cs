using System;
using System.Collections.Generic;
using Tests.Weapons;

namespace Tests.Behaviours.Arms.Weapons
{
    internal interface IArmedWeaponArmBehavioursController<T> where T : IArmedWeaponArmBehaviour
    {
        Action<IWeapon_Obsolete, T> ActivatedAction { get; set; }
        Action<IWeapon_Obsolete, T> UnactivatedAction { get; set; }
        Func<bool> EntryFunc { get; }
        Func<bool> ExitFunc { get; }
        public IReadOnlyDictionary<string, T> Behaviours { get; }

        void ActivateBehaviourBy(IWeapon_Obsolete weapon);
        void UnactivateBehaviourBy(IWeapon_Obsolete weapon);
    }
}