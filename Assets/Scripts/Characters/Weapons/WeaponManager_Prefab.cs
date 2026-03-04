using MNAC.Weapons;
using UnityEngine;

namespace MNAC.Characters.Weapons
{
    [CreateAssetMenu(fileName = "WeaponManager_Prefab", menuName = SOHelper.DEFINITIONS_MENU_NAME + "/WeaponManager_Prefab")]
    internal class WeaponManager_Prefab : WeaponManager
    {
        [SerializeField]
        WeaponPrefabLoader[] _loaders;

        internal override IWeaponLoader[] loaders => _loaders;
    }
}
