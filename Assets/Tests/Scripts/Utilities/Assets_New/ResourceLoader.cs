using NUnit.Framework;
using UnityEngine;

namespace Tests.Utilities.Assets_New
{
    public abstract class ResourceLoader<T> : IResourceLoader<T>
    {
        public abstract T Resource { get; }

        public abstract void Dispose();
        public abstract bool Load();
    }
}
