using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tests.Weapons
{
    [Obsolete]
    public class LauncherDefinitions : MonoBehaviour, ILauncherDefinitions
    {
        [SerializeField]
        GameObject _origin;
        [SerializeField]
        Vector3 _magazinePosition;
        [SerializeField]
        Vector3 _muzzlePosition;
        [SerializeField]
        float _launchRate;
        [SerializeField]
        ushort _ammoSpareQuantity;
        [SerializeField]
        ushort _ammoQuantityInMagazine;
        [SerializeField]
        float _reloadDuration;

        public GameObject AmmoOrigin { get => _origin; }
        public Vector3 MagazinePosition { get => _magazinePosition; }
        public Vector3 MuzzlePosition { get => _muzzlePosition; }
        public float LaunchRate { get => _launchRate; }
        public ushort AmmoTotalQuantity { get => (ushort)(_ammoSpareQuantity + _ammoQuantityInMagazine); }
        public ushort AmmoSpareQuantity { get => _ammoSpareQuantity; }
        public ushort AmmoInMagazineQuantity { get => _ammoQuantityInMagazine; }
        public float ReloadDuration { get => _reloadDuration; }

        //public Dictionary<ushort, IABAssetSaver> AssetSavers => throw new System.NotImplementedException();

        void Start()
        {
            if (_origin != null)
                _origin.SetActive(false);
        }
        private void OnDrawGizmosSelected()
        {
            var pos = this.transform.position;
            var rotation = this.transform.rotation;
            Gizmos.color = Color.yellow;
            var magazinePos = pos + rotation * _magazinePosition;
            Gizmos.DrawCube(magazinePos, Vector3.one * 0.3f);
            Gizmos.DrawLine(magazinePos, magazinePos + this.transform.forward);
            Gizmos.color = Color.red;
            var muzzlePos = pos + rotation * _muzzlePosition;
            Gizmos.DrawCube(muzzlePos, Vector3.one * 0.3f);
            Gizmos.DrawLine(muzzlePos, muzzlePos + this.transform.forward);
        }
    }
}