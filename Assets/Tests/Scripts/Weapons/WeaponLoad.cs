using Tests.Behaviours;
using UnityEngine;
namespace Tests.Weapons
{
    internal class WeaponLoad : MonoBehaviour, ILoad
    {
        [SerializeField]
        GameObject _handle;
        LoadBase _load;

        public GameObject Obj => gameObject;
        void Awake()
        {
            _load = new(gameObject);
            CalculateLocalPositionAndRotation();
        }
        void CalculateLocalPositionAndRotation()
        {
            var pos = transform.position;
            var hpos = _handle.transform.position;
            var r = transform.forward;
            var hr = _handle.transform.forward;

            _load.LocalPosition = -(hpos - pos);
            _load.LocalRotation = Quaternion.FromToRotation(r, hr);

        }

        public void WhenMounted(GameObject mountPoint)
        {
            _load.WhenMounted(mountPoint);
        }

        public void WhenUnmounted(GameObject mountPoint)
        {
            _load.WhenUnmounted(mountPoint);
        }
    }
}