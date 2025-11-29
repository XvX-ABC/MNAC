using UnityEngine;

namespace Tests.Utilities.Assets_New
{
    public abstract class ResourceLoader<T> : MonoBehaviour, IResourceLoader<T>
    {
        public abstract T Resource { get; }

        public abstract void Dispose();
        public abstract bool Load();
    }
}
