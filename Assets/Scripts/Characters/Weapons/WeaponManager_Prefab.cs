using Tests.Weapons_New;
using UnityEngine;

namespace Tests.Characters.Weapons
{
    [CreateAssetMenu(fileName = "WeaponManager_Prefab", menuName = SOHelper.DEFINITIONS_MENU_NAME + "/WeaponManager_Prefab")]
    internal class WeaponManager_Prefab : WeaponManager
    {
        [SerializeField]
        WeaponPrefabLoader[] _loaders;

        internal override IWeaponLoader[] loaders => _loaders;
    }
}
