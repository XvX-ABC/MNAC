using System.Collections.Generic;
using Tests.Assets;
using Tests.Weapons.Launcher;
using UnityEngine;

namespace Tests.Weapons
{
    public class WeaponCore_Obsolete : MonoBehaviour
    {
        Dictionary<SupplyDepotType, ISupplyDepot> _supplyDepots;
        [SerializeField]
        PrefabAssetAgent_Managed[] _originAssets;
        Dictionary<string, GameObject> _cache;
        public WeaponCore_Obsolete()
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
                    result.name = obj.name;
                    return result;
                }

            }
            return result;
        }
        public bool TryGetWeaponOriginObj(string name, out GameObject obj)
        {
            obj = default;
            if (name == null || name.Length == 0)
                return false;
            foreach (var a in _originAssets)
            {
                var d = a.Definitions;
                if (d.Name == name)
                {
                    a.Load();
                    obj = a.Asset;
                    return true;
                }
            }
            return false;
        }
        //public bool TryGetWeaponDescription(string name, out WeaponDescription description)
        //{
        //    description = default;
        //    if (name == null || name.Length == 0)
        //        return false;
        //    if (TryGetWeaponOriginObj(name, out var obj) && obj.TryGetComponent<IWeapon_Obsolete>(out var w))
        //    {
        //        description = new WeaponDescription { Name = w.Name, Type = w.Type };
        //        return true;
        //    }
        //    return false;
        //}
        public bool TryCreateWeaponObj(string name, out GameObject obj)
        {
            obj = default;
            if (name == null || name.Length == 0)
                return false;
            //if (_cache.TryGetValue(name, out var cache))
            //{
            //    obj = cache;
            //    return true;
            //}


            obj = GetObjFromAssets(name);
            //if (obj != null)
            //{
            //    _cache.Add(name, obj);
            //    return true;
            //}

            return true;
            //return false;
        }
        public bool StartSupplyForLauncher(ILauncher_Obsolete launcher)
        {
            if (launcher == null || !_supplyDepots.TryGetValue(SupplyDepotType.Launcher, out var depot))
            {
                return false;
            }
            return depot.StartToSupply(launcher.Fill);
        }
        public bool StopSupplyForLauncher(ILauncher_Obsolete launcher)
        {
            if (launcher == null || !_supplyDepots.TryGetValue(SupplyDepotType.Launcher, out var depot))
                return false;
            return depot.EndToSupply(launcher.Fill);
        }
    }
}
