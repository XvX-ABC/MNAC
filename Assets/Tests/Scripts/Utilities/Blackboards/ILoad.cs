using UnityEngine;

namespace Tests.Utilities.MountPoints
{
    public interface ILoad
    {
        public GameObject Obj { get; }
        public void WhenMounted(GameObject mountPoint);
        public void WhenUnmounted(GameObject mountPoint);
    }
}
