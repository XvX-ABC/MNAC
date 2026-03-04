using System;
using UnityEngine;

namespace MNAC.Weapons
{
    public interface IWeapon
    {
        string Name { get; }
        WeaponType Type { get; }
        GameObject Obj { get; }
    }
}
