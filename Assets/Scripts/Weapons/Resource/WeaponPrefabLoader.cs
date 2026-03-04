using System;
using MNAC.Utilities.Assets;
using UnityEngine;

namespace MNAC.Weapons
{
    [Serializable]
    public class WeaponPrefabLoader : PrefabLoader<Weapon>, IWeaponLoader
    {
        [SerializeField]
        string _name;
        public string Name { get => _name; }
    }

}
