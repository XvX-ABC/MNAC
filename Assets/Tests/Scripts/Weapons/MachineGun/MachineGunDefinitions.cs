using Tests.Assets;
using Tests.Weapons.Launcher;
using UnityEngine;

namespace Tests.Weapons.MachineGuns
{
    public class MachineGunDefinitions : MonoBehaviour, IMachineGunDefinitions
    {
        [SerializeField]
        JsonAssetAgent_Managed<MachineGunNumericalDefinitions> _numericalDefinitionsAsset;
        [SerializeField]
        PrefabAssetAgent_Managed _ammoOriginAsset;
        [SerializeField]
        PrefabAssetAgent_Managed _caseOriginAsset;
        public float PRS => _numericalDefinitionsAsset.Asset.RPS;

        public GameObject AmmoOrigin => _ammoOriginAsset.Asset;
        public GameObject CaseOrigin=>_caseOriginAsset.Asset;
        public Vector3 MagazinePosition => _numericalDefinitionsAsset.Asset.MagazinePosition;

        public Vector3 MuzzlePosition => _numericalDefinitionsAsset.Asset.MuzzlePosition;

        public ushort AmmoTotalQuantity => (ushort)(AmmoSpareQuantity + AmmoTotalQuantity);

        public ushort AmmoSpareQuantity => 0;

        public ushort AmmoInMagazineQuantity => _numericalDefinitionsAsset.Asset.AmmoInMagazineQuantity;

        public float ReloadDurationTime => _numericalDefinitionsAsset.Asset.ReloadDuration;

        public float LaunchDurationTime => 1 / _numericalDefinitionsAsset.Asset.RPS;

        public Vector2 LaunchDelayRange => Vector2.zero;



    }
}
