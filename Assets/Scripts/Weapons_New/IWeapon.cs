using System;
using UnityEngine;

namespace Tests.Weapons_New
{
    public interface IWeapon
    {
        string Name { get; }
        WeaponType Type { get; }
        GameObject Obj { get; }
    }
}
