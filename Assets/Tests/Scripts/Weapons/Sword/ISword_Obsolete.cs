using System;
using System.Numerics;
using System.Reflection;
using Tests.Interaction;
using Tests.Weapons;
using UnityEngine;

namespace Tests.Behaviours.Arms.Weapons.Sword
{
    [Obsolete]
    public interface ISword_Obsolete : IWeapon_Obsolete
    {
        public float SlashRadius { get; }
        public bool EnableDamage { get; set; }
    }
}
