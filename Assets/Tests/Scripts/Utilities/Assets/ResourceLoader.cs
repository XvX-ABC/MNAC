using NUnit.Framework;
using UnityEngine;

namespace Tests.Utilities.Assets
{
    public abstract class ResourceLoader<T> : IResourceLoader<T>
    {
        public abstract T Resource { get; }

        public abstract void Dispose();
        public abstract bool Load();
    }
}
