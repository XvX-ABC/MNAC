//#define EDITOR_ASSET_LOAD
using Assets.Tests.Scripts.Weapons.Assets__v0;
using System;
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
        protected JsonAssetAgent<LauncherNumericalDefinitions> numericalAssetAgent;


        [SerializeField]
        protected GameObjectAssetAgent originAssetAgent;

        protected LauncherNumericalDefinitions numericalDefinitions;
        protected GameObject origin;

        protected virtual void Awake()
        {
            Load();
        }
        public GameObject AmmoOrigin { get => origin; set => origin = value; }
        public Vector3 MagazinePosition { get => numericalDefinitions.MagazinePosition; set => numericalDefinitions.MagazinePosition = value; }
        public Vector3 MuzzlePosition
        {
            get => numericalDefinitions.MuzzlePosition;
            set => numericalDefinitions.MuzzlePosition = value;
        }
        public ushort AmmoTotalQuantity { get => (ushort)(AmmoSpareQuantity + AmmoInMagazineQuantity); }

        public ushort AmmoSpareQuantity { get => numericalDefinitions.AmmoSpareQuantity; set => numericalDefinitions.AmmoSpareQuantity = value; }
        public ushort AmmoInMagazineQuantity { get => numericalDefinitions.AmmoInMagazineQuantity; set => numericalDefinitions.AmmoInMagazineQuantity = value; }
        public float ReloadDuration { get => numericalDefinitions.ReloadDuration; set => numericalDefinitions.ReloadDuration = value; }
        public float LaunchDurationTime { get => numericalDefinitions.LaunchDurationTime; set => numericalDefinitions.LaunchDurationTime = value; }
        public Vector2 LaunchDelayRange { get => numericalDefinitions.LaunchDelayRange; set => numericalDefinitions.LaunchDelayRange = value; }
        public Dictionary<string, AssetDefinitions> AssetDefinitionsMap { get => throw new NotImplementedException(); }
        public AssetDefinitions OriginAssetDefinitions { get => originAssetAgent.Definitions; set => originAssetAgent.Definitions = value; }
        public AssetDefinitions NumericalAssetDefinitions { get => numericalAssetAgent.Definitions; set => numericalAssetAgent.Definitions = value; }

        protected virtual void LoadOrigin()
        {
            origin = originAssetAgent.Load();
        }
        protected virtual void LoadNumericalDefinitions()
        {
            //_definitions = definitionsAssetLoader.Load();
            numericalDefinitions = numericalAssetAgent.Load() ?? new();
        }
        protected void TryLoadDefinitions()
        {
            if (numericalDefinitions == null)
                LoadNumericalDefinitions();
        }
        public virtual void Save()
        {
            numericalAssetAgent.Save(numericalDefinitions);
            originAssetAgent.Save(origin);
        }
        public virtual void Load()
        {
            LoadNumericalDefinitions();
            LoadOrigin();
        }
    }
}
