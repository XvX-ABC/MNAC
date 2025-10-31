using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Tests.Assets
{
    public class AssetAgentBase<T> : IAssetAgent<T>
    {
        [SerializeField]
        protected AssetDefinitions definitions;
        protected IAssetLoader<T> loader;
        public IAssetLoader Loader { get => loader; }
#if UNITY_EDITOR

        protected IAssetSaver<T> _saver;
        public IAssetSaver Saver { get => _saver; }
        public AssetDefinitions Definitions
        {
            get => definitions;
            set
            {
                if (value != null)
                    definitions = value;
            }
        }
        protected AssetAgentBase(IAssetLoader<T> loader, IAssetSaver<T> saver)
        {
            this.loader = loader;
            _saver = saver;
        }
        public virtual void Save(T obj)
        {
            var path = definitions.GetPath();
            _saver.SavePath = path;
            _saver.BundleName = definitions.BundleName;
            _saver.Save(obj);
        }

#else
        public AssetAgentBase(IAssetLoader<T> loader)
        {
            _loader = loader;
        }
#endif

        public virtual T Load()
        {
            try
            {

                var path = definitions.GetPath();
                loader.Path = path;
#if UNITY_EDITOR && EDITOR_ASSET_LOAD
                path = Path.Join("Assets", loader.Path);
                var importer = AssetImporter.GetAtPath(path);
                if (importer != null)
                    definitions.BundleName = importer.assetBundleName;
#endif
                return loader.Load();
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
                return default;
            }
        }

    }
}
