using System;

namespace Tests.Utilities.Assets_New
{
    public interface IResourceLoader<T> : IDisposable
    {
        public T Resource { get; }
        public bool Load();
    }
}
