using UnityEngine;

namespace Tests.Weapons
{
    public class LauncherDefines : MonoBehaviour, ILauncherDefines
    {
        [SerializeField]
        GameObject _origin;
        [SerializeField]
        Vector3 _borePosition;
        [SerializeField]
        Vector3 _muzzlePosition;
        [SerializeField]
        float _firingRate;
        [SerializeField]
        ushort _bulletsTotalNum;
        [SerializeField]
        ushort _bulletsTotalNumInMagazine;
        [SerializeField]
        float _reloadDuration;

        public GameObject ProjectileOrigin { get => _origin; }
        public Vector3 BorePosition { get => _borePosition; }
        public Vector3 MuzzlePosition { get => _muzzlePosition; }
        public float FiringRate { get => _firingRate; }
        public ushort BulletsTotalNum { get => _bulletsTotalNum; }
        public ushort BulletsTotalNumInMagazine { get => _bulletsTotalNumInMagazine; }
        public float ReloadDuration { get => _reloadDuration; }
        void Start()
        {
            _origin.SetActive(false);
        }
        private void OnDrawGizmosSelected()
        {
            var pos = this.transform.position;
            var rotation = this.transform.rotation;
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(pos + this.transform.InverseTransformPoint(_borePosition), Vector3.one * 0.3f);
            Gizmos.color = Color.red;
            Gizmos.DrawCube(pos + this.transform.InverseTransformPoint(_muzzlePosition), Vector3.one * 0.3f);
        }
    }
}