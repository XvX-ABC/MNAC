using System;
using System.Collections.Generic;
using MNAC.Weapons;
using MNAC.Weapons;

namespace MNAC.Behaviours.Arms.Weapons
{
    internal interface IArmedArmBehavioursController<T> where T : IArmedArmBehaviour
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