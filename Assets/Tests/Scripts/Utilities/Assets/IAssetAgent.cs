using Unity.VisualScripting;

namespace Tests.Assets
{
    public interface IAssetAgent<T>
    {
        IAssetLoader Loader { get; }
        T Load();
#if UNITY_EDITOR
        IAssetSaver Saver { get; }

        void Save(T obj);
#endif
    }


    public interface IAssetAgent_Managed
    {
        public object Asset { get; set; }
        public void Load();
#if UNITY_EDITOR
        public void Save();
#endif

    }
    public interface IAssetAgent_Managed<T> : IAssetAgent<T>
    {
        public T Asset { get; set; }
        public new void Load();
#if UNITY_EDITOR
        public void Save();
#endif
    }
}