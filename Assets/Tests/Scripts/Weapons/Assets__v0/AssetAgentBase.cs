using Assets.Tests.Scripts.Weapons.Assets__0;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Assets.Tests.Scripts.Weapons.Assets__v0
{
    public class AssetAgentBase<T> : IAssetAgent<T>
    {
        [SerializeField]
        protected AssetDefinitions definitions;
        protected IAssetLoader<T> _loader;
        public IAssetLoader Loader { get => _loader; }
#if UNITY_EDITOR

        protected IAssetSaver<T> _saver;
        public IAssetSaver Saver { get => _saver; }
        protected AssetAgentBase(IAssetLoader<T> loader, IAssetSaver<T> saver)
        {
            _loader = loader;
            _saver = saver;
        }
        public virtual void Save(T obj)
        {
            _saver.SavePath = definitions.GetPath();
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


            _loader.Path = definitions.GetPath();
#if UNITY_EDITOR && EDITOR_ASSET_LOAD
            var path = Path.Join("Assets", _loader.Path);
            var importer = AssetImporter.GetAtPath(path);
            if (importer != null)
                definitions.BundleName = importer.assetBundleName;
#endif
            return _loader.Load();
        }

    }
}
