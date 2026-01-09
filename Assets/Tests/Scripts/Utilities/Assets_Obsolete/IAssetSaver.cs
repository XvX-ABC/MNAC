using System;
using UnityEngine;
namespace Tests.Obsolete_Assets
{
    public interface IAssetSaver
    {
        public string SavePath { get; set; }
        public string BundleName { get; set; }
        public bool Save(object obj);
    }
    public interface IAssetSaver<T> : IAssetSaver
    {
        public bool Save(T obj);
    }
    public interface IABAssetSaver : IAssetSaver
    {
    }
    public interface IABAssetSaver<T> : IAssetSaver<T>, IABAssetSaver
    {

    }
    public abstract class AssetSaverBase<T> : IABAssetSaver<T>
    {
        protected string savePath;
        protected string bundleName;
        public virtual string SavePath
        {
            get => savePath;
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                if (value.Length == 0)
                    Debug.LogWarning("The value to assign to the save path can't is empty.");
                savePath = value;
            }
        }
        public virtual string BundleName
        {
            get => bundleName;
            set => bundleName = value;


        }

        public abstract bool Save(T obj);
        public abstract bool Save(object obj);
    }
}
