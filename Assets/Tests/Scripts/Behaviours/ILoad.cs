using UnityEngine;

namespace Tests.Behaviours
{
    public interface ILoad
    {
        public GameObject Obj { get; }
        public void WhenMounted(GameObject mountPoint);
        public void WhenUnmounted(GameObject mountPoint);
    }
}
