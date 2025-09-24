using UnityEngine;

namespace Tests.Weapons.MachineGuns
{
    public class MachineGunDefinitions : MonoBehaviour, IMachineGunDefinitions
    {
        [SerializeField]
        float _PRS;
        [SerializeField]
        GameObject _ammoOrigin;
        [SerializeField]
        GameObject _caseOrigin;
        [SerializeField]
        Vector3 _magazinePosition;
        [SerializeField]
        Vector3 _muzzlePosition;
        [SerializeField]
        ushort _ammoReservesQuantity;
        [SerializeField]
        ushort _ammoInMagazineQuantity;
        [SerializeField]
        float _reloadDurationTime;

        public float PRS => _PRS;

        public GameObject CaseOrigin => _caseOrigin;

        public GameObject AmmoOrigin => _ammoOrigin;

        public Vector3 MagazinePosition => _magazinePosition;

        public Vector3 MuzzlePosition => _muzzlePosition;

        public ushort AmmoTotalQuantity => (ushort)(AmmoInMagazineQuantity + AmmoReservesQuantity);

        public ushort AmmoReservesQuantity => _ammoReservesQuantity;

        public ushort AmmoInMagazineQuantity => _ammoInMagazineQuantity;

        public float ReloadDurationTime => _reloadDurationTime;

        public float LaunchDurationTime => 1 / _PRS;

        public Vector2 LaunchDelayRange => Vector2.zero;
    }
}
