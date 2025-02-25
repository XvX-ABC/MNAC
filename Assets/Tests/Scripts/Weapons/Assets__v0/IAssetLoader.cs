using System;
using UnityEditor;
using Object = UnityEngine.Object;
namespace Assets.Tests.Scripts.Weapons.Assets__v0
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
