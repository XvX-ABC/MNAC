using Assets.Tests.Scripts.Weapons.Assets;
namespace Assets.Tests.Scripts.Weapons.Assets__v0
{
#if UNITY_EDITOR
    public interface IAssetSaver
    {
        public string SavePath { get; set; }
        public string ABPath { get; set; }
        public bool Save(object obj);
    }
    public interface IAssetSaver<T> : IAssetSaver
    {
        public bool Save(T obj);
    }
    public interface IABAssetSaver : IAssetSaver
    {
        public string ABPath { get; set; }
    }
    public interface IABAssetSaver<T> : IAssetSaver<T>, IABAssetSaver
    {

    }
    public abstract class AssetSaverBase<T> : IABAssetSaver<T>
    {
        protected string savePath;
        protected string abPath;
        public virtual string SavePath { get => savePath; set => savePath = value; }
        public virtual string ABPath { get => abPath; set => abPath = value; }

        public abstract bool Save(T obj);
        public abstract bool Save(object obj);
    }
#endif
}
