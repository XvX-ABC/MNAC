using System;
using Tests.Utilities.Assets;
using UnityEngine;

namespace Tests.Weapons_New
{
    [Serializable]
    public class WeaponPrefabLoader : PrefabLoader<Weapon>, IWeaponLoader
    {
        [SerializeField]
        string _name;
        public string Name { get => _name; }
    }

}
