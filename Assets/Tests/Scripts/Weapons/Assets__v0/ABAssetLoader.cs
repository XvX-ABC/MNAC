using Assets.Scripts.Utilities.Assets;
using System;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;
namespace Assets.Tests.Scripts.Weapons.Assets__v0
{
    [Serializable]
    public class ABAssetLoader<T> : IAssetLoader<T> where T : Object
    {
        [SerializeField]
        protected string bundlePath;
        [SerializeField]
        protected string resourceName;
        protected ABLoader abLoader;
        public string Path
        {
            get
            {
                return System.IO.Path.Combine(bundlePath, resourceName);
            }
            set
            {
                if (value.Length == 0)
                {
                    bundlePath = "";
                    resourceName = "";
                }
                else
                {
                    bundlePath = System.IO.Path.GetDirectoryName(value);
                    resourceName = System.IO.Path.GetFileName(value);
                }
            }
        }
        public ABAssetLoader()
        {
            abLoader = ABLoader.Instance;
        }
        public T Load()
        {
            if (bundlePath == null || resourceName == null)
            {
                Debug.LogWarning($"Can't to load the asset, because the bundle name or resource name is null.");
                return null;
            }
            var asset = abLoader.LoadResource<T>(bundlePath, resourceName);
            if (asset == null)
                Debug.LogWarning($"Load asset at path '{Path}' failed.");
            return asset;

        }
        object IAssetLoader.Load()
        {
            return this.Load();
        }
    }
}
