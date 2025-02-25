using System;
using Object = UnityEngine.Object;

namespace Assets.Tests.Scripts.Weapons.Assets
{
    public interface IAssetLoader<T> where T : Object
    {
        public string LoadPath { get; set; }
        public T Load();
    }
    public interface IABAssetAgent
    {
        public string BundlePath { get; set; }
        public string ResourceName { get; set; }
        public Object Asset { get; }
    }
    public interface IABAssetAgent<T> : IABAssetAgent, IAssetLoader<T> where T : Object
    {
        public new T Asset { get; }
    }
}
