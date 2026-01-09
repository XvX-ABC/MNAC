using System;
using UnityEngine;
namespace Tests.Obsolete_Assets
{
    [Serializable]
    public class TextAssetLoader : IAssetLoader<string>
    {
#if UNITY_EDITOR && EDITOR_ASSET_LOAD  
        EditorAssetLoader<TextAsset> _loader;
        public TextAssetLoader()
        {
            _loader = new();
        }
#else
        ABAssetLoader<TextAsset> _loader;
        public TextAssetLoader()
        {
            _loader = new();
        }
#endif


        public string Path { get => _loader.Path; set => _loader.Path = value; }

        public string Load()
        {
            var asset = _loader.Load();
            if (asset == null)
                return "";
            return asset.text;
        }

        object IAssetLoader.Load()
        {
            return Load();
        }
    }
}
