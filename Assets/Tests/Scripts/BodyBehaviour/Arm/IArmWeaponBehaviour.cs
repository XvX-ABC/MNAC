using System;
using Tests.Weapons;
using UnityEngine;

namespace Tests.BodyBehaviour.Arm
{
    public interface IArmWeaponBehaviour : IArmBehaviour
    {
        //[Obsolete]
        //public GameObject WeaponObj { get; set; }
        public WeaponType Type { get; }
        public IWeapon Weapon { get; set; }

    }
}
