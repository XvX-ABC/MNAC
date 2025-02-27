using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using ath = System.IO.Path;
namespace Assets.Tests.Scripts.Weapons.Assets__v0
{
#if UNITY_EDITOR
    [Serializable]
    public class EditorAssetLoader<T> : IAssetLoader<T> where T : Object
    {
        [SerializeField]
        protected string loadPath;

        public string Path
        {
            get => loadPath;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (value == "")
                    Debug.LogWarning("The value assign to the load path can't is empty.");
                loadPath = value;
            }
        }
        public T Load()
        {
            if (loadPath == null)
            {
                Debug.LogWarning($"Can't to load the asset, because the load path is null.");
                return null;
            }
            var path = System.IO.Path.Join("Assets", loadPath);
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset == null)
                Debug.LogWarning($"Load asset at path '{path}' failed.");
            return asset;
        }

        object IAssetLoader.Load()
        {
            return Load();
        }
    }
#endif
}
