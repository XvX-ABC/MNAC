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
}