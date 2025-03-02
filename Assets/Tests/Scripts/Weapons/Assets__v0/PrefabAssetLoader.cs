//#define EDITOR_ASSET_LOAD
using System;
using UnityEditor;
using UnityEngine;
using ath = System.IO.Path;
namespace Assets.Tests.Scripts.Weapons.Assets__v0
{
    public class PrefabAssetLoader : IAssetLoader<GameObject>
    {
#if UNITY_EDITOR && EDITOR_ASSET_LOAD
        EditorAssetLoader<GameObject> _loader;
        public PrefabAssetLoader()
        {
            _loader = new();
        }
#else
        ABAssetLoader<GameObject> _loader;
        public PrefabAssetLoader()
        {
            _loader = new();
        }
#endif
        public string Path
        {
            get => _loader.Path;
            set
            {
                if (value == null || value.Length == 0)
                    throw new ArgumentException("The load path can't assign a empty value. ");
                var p = value;
#if UNITY_EDITOR && EDITOR_ASSET_LOAD
                if (ath.GetExtension(p) != "prefab")
                    p = ath.ChangeExtension(p, "prefab");
#endif
                _loader.Path = p;
            }
        }

        public GameObject Load()
        {
            if (Path == null || Path.Length == 0)
                return null;
            var obj = _loader.Load();
#if UNITY_EDITOR
            if (obj != null)
            {
                var path = AssetDatabase.GetAssetPath(obj);
                _loader.Path = path;
            }
#endif
            return obj;
        }

        object IAssetLoader.Load()
        {
            return this.Load();
        }
    }
}
