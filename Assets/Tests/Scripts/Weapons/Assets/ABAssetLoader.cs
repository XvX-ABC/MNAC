#define EDITOR_ASSET_LOAD
using Assets.Scripts.Utilities.Assets;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Tests.Scripts.Weapons.Assets
{
#if UNITY_EDITOR
    public class EditorAssetLoader<T> : IAssetLoader<T> where T : Object
    {
        string _loadPath;
        public EditorAssetLoader(string loadPath)
        {
            _loadPath = loadPath;
        }

        public string LoadPath { get => _loadPath; set => _loadPath = value; }

        public T Load()
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(_loadPath);
            if (asset == null)
                Debug.LogWarning($"Load asset at path '{_loadPath}' failed.");
            return asset;
        }
    }

#endif
    public abstract class AssetAgent<T> where T : Object
    {
        protected T asset;
        protected IAssetLoader<T> assetLoader;
        public T Asset
        {
            get
            {
                if (asset == null)
                    asset = assetLoader.Load();
                return asset;
            }
        }
        public string LoadPath
        {
            get => assetLoader.LoadPath;
            set => assetLoader.LoadPath = value;
        }
        protected IABAssetSaver assetSaver;
        protected AssetAgent(string assetLoadPath, IABAssetSaver saver)
        {
            assetLoader = new EditorAssetLoader<T>(assetLoadPath);
            assetSaver = saver;
        }
        protected AssetAgent(IABAssetSaver saver) : this(null, saver)
        {
        }
        public virtual void Save()
        {
            assetSaver.Save();
        }
    }
    public class JsonAssetAgent : AssetAgent<TextAsset>
    {
        public JsonAssetAgent(string abPath, Func<object> targetGetFunc) : base(abPath, new JsonAssetBindSaver(abPath, targetGetFunc))
        {

        }
    }
    public class ObjectAssetAgent : AssetAgent<GameObject>
    {
        Func<GameObject> _func;
        GameObject CustomSave()
        {
            var obj = _func();
            assetLoader.LoadPath = AssetDatabase.GetAssetPath(obj);
            return obj;
        }
        public ObjectAssetAgent(string abPath, Func<GameObject> getFunc) : base(null)
        {
            _func = getFunc;
            assetSaver = new ObjectAssetBindSaver(abPath, CustomSave);
        }


        public ObjectAssetAgent(string abPath, string assetLoadPath, Func<GameObject> getFunc) : base(assetLoadPath, new ObjectAssetBindSaver(abPath, getFunc))
        {

        }
        public override void Save()
        {
            base.Save();
        }

    }
    [Serializable]
    public class ABAssetLoader<T> : IABAssetAgent<T> where T : Object
    {
        [SerializeField]
        protected string bundlePath;
        [SerializeField]
        protected string resourceName;
        protected ABLoader instance;
        protected T asset;

        public string BundlePath { get => bundlePath; set => bundlePath = value; }
        public string ResourceName { get => resourceName; set => resourceName = value; }
        Object IABAssetAgent.Asset
        {
            get
            {
                if (asset == null)
                    asset = Load();
                return asset;
            }
        }
        public T Asset
        {
            get
            {
                if (asset == null)
                    asset = Load();
                return asset;
            }
        }

        public string LoadPath
        {
            get
            {
                return Path.Combine(bundlePath, resourceName);
            }
            set
            {
                bundlePath = Path.GetDirectoryName(value);
                resourceName = Path.GetFileName(value);
            }
        }

        public ABAssetLoader()
        {
            instance = ABLoader.Instance;
        }
        public ABAssetLoader(string bundleName, string resourceName) : this()
        {
            this.bundlePath = bundleName;
            this.resourceName = resourceName;
        }
        public T Load()
        {
            if (bundlePath == null || resourceName == null)
            {
                Debug.LogWarning($"Can't to load the asset, because the bundle name or resource name is null.");
                return null;
            }
            var asset = instance.LoadResource<T>(BundlePath, ResourceName);
            return asset;
        }

    }
}
