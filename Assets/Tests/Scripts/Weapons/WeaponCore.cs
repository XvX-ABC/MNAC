using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Assets;
using Tests.Weapons;
using Tests.Weapons.Launcher;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Assets.Tests.Scripts.Weapons
{
    public class WeaponOriginNotContainsException : Exception
    {
        public WeaponOriginNotContainsException(WeaponCore core, string name) : base($"There is not exists a weapon origin which's name is '{name}' in the weapon core '{core.name}'")
        {

        }
    }
    public class GetWeaponObjByNameFailedException : Exception
    {
        public GetWeaponObjByNameFailedException(string name) : base($"Get a weapon obj by the name '{name}'  failed.")
        {

        }
    }
    public class WeaponCore : MonoBehaviour
    {
        Dictionary<SupplyDepotType, ISupplyDepot> _supplyDepots;
        [SerializeField]
        PrefabAssetAgent_Managed[] _originAssets;
        Dictionary<string, GameObject> _cache;
        public WeaponCore()
        {

            _supplyDepots = new();
            _cache = new();
        }
        public bool ContainsOrigin(string name)
        {
            foreach (var a in _originAssets)
            {
                var d = a.Definitions;
                if (d.Name == name)
                    return true;
            }
            return false;
        }
        protected GameObject GetObjFromAssets(string name)
        {
            var result = default(GameObject);
            foreach (var a in _originAssets)
            {
                var d = a.Definitions;
                if (d.Name == name)
                {
                    a.Load();
                    var obj = a.Asset;
                    result = Instantiate(obj);
                    return result;
                }

            }
            return result;
        }
        public bool TryGetWeaponOrigin(string name, out GameObject origin)
        {
            origin = default;
            if (name == null || name.Length == 0)
                return false;
            foreach (var a in _originAssets)
            {
                var d = a.Definitions;
                if (d.Name == name)
                {
                    a.Load();
                    origin = a.Asset;
                    return true;
                }
            }
            return false;
        }
        public bool TryGetWeaponObj(string name, out GameObject obj)
        {
            obj = default;
            if (name == null || name.Length == 0)
                return false;
            if (_cache.TryGetValue(name, out var cache))
            {
                obj = cache;
                return true;
            }


            obj = GetObjFromAssets(name);
            if (obj != null)
            {
                _cache.Add(name, obj);
                return true;
            }

            return false;
        }
        public bool StartSupplyForLauncher(ILauncher launcher)
        {
            if (launcher == null || !_supplyDepots.TryGetValue(SupplyDepotType.Launcher, out var depot))
            {
                return false;
            }
            return depot.StartToSupply(launcher.Fill);
        }
        public bool StopSupplyForLauncher(ILauncher launcher)
        {
            if (launcher == null || !_supplyDepots.TryGetValue(SupplyDepotType.Launcher, out var depot))
                return false;
            return depot.EndToSupply(launcher.Fill);
        }
    }
}
