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

namespace Assets.Tests.Scripts.Weapons.Assets
{
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
        protected ABAssetLoader<GameObject> ammoOriginAssetAgent;
        [SerializeField]
        protected ABAssetLoader<TextAsset> definitionsAssetAgent;

        Definitions _definitions;
        protected GameObject origin;
        protected void Awake()
        {

        }

        public GameObject AmmoOrigin
        {
            get
            {
                if (origin == null)
                {
                    origin = ammoOriginAssetAgent.Asset;
                }
                return origin;
            }
        }

        public Vector3 MagazinePosition
        {
            get
            {
                TryLoadDefinitions();
                return _definitions.MagazinePosition;
            }
        }

        public Vector3 MuzzlePosition
        {
            get
            {
                TryLoadDefinitions();
                return _definitions.MuzzlePosition;

            }
        }

        public float LaunchRate
        {
            get
            {
                TryLoadDefinitions();
                return _definitions.LaunchRate;

            }
        }

        public ushort AmmoTotalQuantity
        {
            get
            {
                return (ushort)(AmmoSpareQuantity + AmmoQuantityInMagazine);
            }
        }

        public ushort AmmoSpareQuantity
        {
            get
            {
                TryLoadDefinitions();
                return _definitions.AmmoSpareQuantity;

            }
        }

        public ushort AmmoQuantityInMagazine
        {
            get
            {
                TryLoadDefinitions();
                return _definitions.AmmoQuantityInMagazine;

            }
        }

        public float ReloadDuration
        {
            get
            {
                TryLoadDefinitions();
                return _definitions.ReloadDuration;

            }
        }
        protected virtual void LoadDefinitions()
        {
            var asset = definitionsAssetAgent.Asset;
            if (asset != null)
            {
                var text = asset.text;
                _definitions = JsonUtility.FromJson<Definitions>(text);
            }
        }
        protected void TryLoadDefinitions()
        {
            if (_definitions == null)
                LoadDefinitions();
        }
    }
}
