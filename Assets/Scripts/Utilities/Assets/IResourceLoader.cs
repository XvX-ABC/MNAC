using System;

namespace MNAC.Utilities.Assets
{
    public interface IResourceLoader<T> : IDisposable
    {
        public T Resource { get; }
        public bool Load();
    }
}
