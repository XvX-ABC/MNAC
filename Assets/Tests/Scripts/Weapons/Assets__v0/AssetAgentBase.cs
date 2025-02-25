using Assets.Tests.Scripts.Weapons.Assets__0;
using System.IO;
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
        public void Save(T obj)
        {
            _saver.SavePath = definitions.FilePath;
            _saver.ABPath = definitions.BundleName;
            _saver.Save(obj);
        }

#else
        public AssetAgent(IAssetLoader<T> loader)
        {
            _loader = loader;
        }
#endif

        public virtual T Load()
        {
            _loader.Path = definitions.GetPath();
            return _loader.Load();
        }

    }
}
