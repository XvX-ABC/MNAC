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
    [Serializable]
    public class AssetDefinitions
    {
        public string BundleName;
#if UNITY_EDITOR
        public string FilePath;
#endif
        public string GetPath()
        {
#if UNITY_EDITOR && EDITOR_ASSET_LOAD
            return FilePath;
#else
            return BundleName;
#endif
        }
    }
    public class LauncherDefinitions_AB : MonoBehaviour, ILauncherDefinitions
    {
        [Serializable]
        public class Definitions
        {
            public Vector3 MagazinePosition;
            public Vector3 MuzzlePosition;
            public float LaunchRate;
            public ushort AmmoSpareQuantity;
            public ushort AmmoQuantityInMagazine;
            public float ReloadDuration;
        }


        [SerializeField]
        protected JsonAssetAgent<Definitions> definitionsAssetAgent;
        [SerializeField]
        protected GameObjectAssetAgent originAssetAgent;

        Definitions _definitions;
        protected GameObject origin;
        void Awake()
        {
            LoadOrigin();
            LoadDefinitions();
        }
        public GameObject AmmoOrigin { get => origin; }
        public Vector3 MagazinePosition { get => _definitions.MagazinePosition; }
        public Vector3 MuzzlePosition { get => _definitions.MuzzlePosition; }
        public float LaunchRate { get => _definitions.LaunchRate; }
        public ushort AmmoTotalQuantity { get => (ushort)(AmmoSpareQuantity + AmmoQuantityInMagazine); }

        public ushort AmmoSpareQuantity { get => _definitions.AmmoSpareQuantity; }
        public ushort AmmoQuantityInMagazine { get => _definitions.AmmoQuantityInMagazine; }
        public float ReloadDuration { get => _definitions.ReloadDuration; }
        protected virtual void LoadOrigin()
        {
            origin = originAssetAgent.Load();
        }
        protected virtual void LoadDefinitions()
        {
            //_definitions = definitionsAssetLoader.Load();
            _definitions = definitionsAssetAgent.Load();
        }
        protected void TryLoadDefinitions()
        {
            if (_definitions == null)
                LoadDefinitions();
        }
    }
}
