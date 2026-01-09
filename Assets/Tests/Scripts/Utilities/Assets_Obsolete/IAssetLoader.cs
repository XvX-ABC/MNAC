using System;
using UnityEditor;
using Object = UnityEngine.Object;
namespace Tests.Obsolete_Assets
{
    public interface IAssetLoader
    {
        public string Path { get; set; }
        public object Load();
    }
    public interface IAssetLoader<T> : IAssetLoader
    {
        public new T Load();
    }
}
