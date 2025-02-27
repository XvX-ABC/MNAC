//#define EDITOR_ASSET_LOAD
using Assets.Tests.Scripts.Weapons.Assets__v0;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Weapons;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Tests.Scripts.Weapons.Assets__0
{
    public class LauncherDefinitions_AB : MonoBehaviour, ILauncherDefinitions, ILauncherDefinitionsEditor
    {
        [SerializeField]
        protected JsonAssetAgent<LauncherNumericalDefinitions> definitionsAssetAgent;


        [SerializeField]
        protected GameObjectAssetAgent originAssetAgent;

        LauncherNumericalDefinitions _numericalDefinitions;
        protected GameObject origin;
        void Awake()
        {
            LoadOrigin();
            LoadNumericalDefinitions();
        }
        public GameObject AmmoOrigin { get => origin; set => origin = value; }
        public Vector3 MagazinePosition { get => _numericalDefinitions.MagazinePosition; set => _numericalDefinitions.MagazinePosition = value; }
        public Vector3 MuzzlePosition { get => _numericalDefinitions.MuzzlePosition; set => _numericalDefinitions.MuzzlePosition = value; }
        public ushort AmmoTotalQuantity { get => (ushort)(AmmoSpareQuantity + AmmoInMagazineQuantity); }

        public ushort AmmoSpareQuantity { get => _numericalDefinitions.AmmoSpareQuantity; set => _numericalDefinitions.AmmoSpareQuantity = value; }
        public ushort AmmoInMagazineQuantity { get => _numericalDefinitions.AmmoQuantityInMagazine; set => _numericalDefinitions.AmmoQuantityInMagazine = value; }
        public float ReloadDuration { get => _numericalDefinitions.ReloadDuration; set => _numericalDefinitions.ReloadDuration = value; }
        public float LaunchDurationTime { get => _numericalDefinitions.LaunchDurationTime; set => _numericalDefinitions.LaunchDurationTime = value; }
        public Vector2 LaunchDelayRange { get => _numericalDefinitions.LaunchDelayRange; set => _numericalDefinitions.LaunchDelayRange = value; }

        protected virtual void LoadOrigin()
        {
            origin = originAssetAgent.Load();
        }
        protected virtual void LoadNumericalDefinitions()
        {
            //_definitions = definitionsAssetLoader.Load();
            _numericalDefinitions = definitionsAssetAgent.Load();
        }
        protected void TryLoadDefinitions()
        {
            if (_numericalDefinitions == null)
                LoadNumericalDefinitions();
        }
        public void Save()
        {
            definitionsAssetAgent.Save(_numericalDefinitions == null ? new() : _numericalDefinitions);
            originAssetAgent.Save(origin);
        }
        public void Load()
        {
            LoadNumericalDefinitions();
            LoadOrigin();
        }
    }
}
