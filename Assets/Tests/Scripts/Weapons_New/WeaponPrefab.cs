using System;
using UnityEngine;

namespace Tests.Weapons_New
{
    [Serializable]
    public class WeaponPrefab : WeaponSource
    {
        [SerializeField]
        Weapon _prefab;
        IWeapon _weapon;
        public override IWeapon Weapon => _weapon;
        public override string Name => _prefab.name;
        public override void Dispose()
        {
        }

        public override void Initialize()
        {
            _weapon = GameObject.Instantiate(_prefab);

        }
    }
}
