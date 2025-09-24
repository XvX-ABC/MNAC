//#define EDITOR_ASSET_LOAD
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Assets;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Tests.Weapons.Launcher
{
    public class LauncherDefinitions_AB : MonoBehaviour, ILauncherDefinitions, ILauncherDefinitionsEditor
    {
        [SerializeField]
        protected JsonAssetAgent<LauncherNumericalDefinitions> numericalAssetAgent;


        [SerializeField]
        protected PrefabAssetAgent originAssetAgent;

        protected LauncherNumericalDefinitions numericalDefinitions;
        protected GameObject origin;

        protected virtual void Awake()
        {
            Load();
        }
        public virtual GameObject AmmoOrigin { get => origin; set => origin = value; }
        public virtual Vector3 MagazinePosition { get => numericalDefinitions.MagazinePosition; set => numericalDefinitions.MagazinePosition = value; }
        public virtual Vector3 MuzzlePosition
        {
            get => numericalDefinitions.MuzzlePosition;
            set => numericalDefinitions.MuzzlePosition = value;
        }
        public ushort AmmoTotalQuantity { get => (ushort)(AmmoReservesQuantity + AmmoInMagazineQuantity); }

        public virtual ushort AmmoReservesQuantity { get => numericalDefinitions.AmmoSpareQuantity; set => numericalDefinitions.AmmoSpareQuantity = value; }
        public virtual ushort AmmoInMagazineQuantity { get => numericalDefinitions.AmmoInMagazineQuantity; set => numericalDefinitions.AmmoInMagazineQuantity = value; }
        public virtual float ReloadDurationTime { get => numericalDefinitions.ReloadDuration; set => numericalDefinitions.ReloadDuration = value; }
        public virtual float LaunchDurationTime { get => numericalDefinitions.LaunchDurationTime; set => numericalDefinitions.LaunchDurationTime = value; }
        public virtual Vector2 LaunchDelayRange
        {
            get => numericalDefinitions.LaunchDelayRange;
            //set => numericalDefinitions.LaunchDelayRange = value;
            set
            {
                var x = value.x;
                var y = Mathf.Max(x, value.y);
                numericalDefinitions.LaunchDelayRange = new Vector2(x, y);
            }
        }
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
