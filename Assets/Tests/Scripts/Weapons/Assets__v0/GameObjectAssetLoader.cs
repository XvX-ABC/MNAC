//#define EDITOR_ASSET_LOAD
using System;
using UnityEditor;
using UnityEngine;
namespace Assets.Tests.Scripts.Weapons.Assets__v0
{
    public class GameObjectAssetLoader : IAssetLoader<GameObject>
    {
#if UNITY_EDITOR && EDITOR_ASSET_LOAD
        EditorAssetLoader<GameObject> _loader;
        public GameObjectAssetLoader()
        {
            _loader = new();
        }
#else
        ABAssetLoader<GameObject> _loader;
        public GameObjectAssetLoader()
        {
            _loader = new();
        }
#endif
        public string Path { get => _loader.Path; set => _loader.Path = value; }

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
