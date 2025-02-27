using System;
using UnityEngine;
using ath = System.IO.Path;
namespace Assets.Tests.Scripts.Weapons.Assets__v0
{
    [Serializable]
    public class JsonAssetLoader<T> : IAssetLoader<T>
    {
        internal TextAssetLoader loader;
        public JsonAssetLoader()
        {
            loader = new TextAssetLoader();
        }
        public string Path
        {
            get => loader.Path;
            set
            {
                var p = value;
#if UNITY_EDITOR && EDITOR_ASSET_LOAD
                if (ath.GetExtension(p) != "json")
                    p = ath.ChangeExtension(p, "json");
#endif
                loader.Path = p;
            }
        }

        public T Load()
        {
            var text = loader.Load();
            var obj = (T)JsonUtility.FromJson(text, typeof(T));
            return obj;
        }

        object IAssetLoader.Load()
        {
            return this.Load();
        }
    }
}
