using System;
using MNAC.Utilities.MountPoints;
using UnityEngine;

namespace MNAC.Utilities.MountPoints
{
    public class LoadBase : ILoad
    {
        GameObject _obj;
        Vector3 _localPosition;
        Quaternion _localRotation;

        public LoadBase(GameObject obj)
        {
            _obj = obj ?? throw new ArgumentNullException(nameof(obj));
        }

        public GameObject Obj => _obj;

        public Vector3 LocalPosition { get => _localPosition; set => _localPosition = value; }
        public Quaternion LocalRotation { get => _localRotation; set => _localRotation = value; }

        public void WhenMounted(GameObject mountPoint)
        {
            _obj.transform.localPosition = _localPosition;
            //_obj.transform.localRotation = _localRotation;
        }

        public void WhenUnmounted(GameObject mountPoint)
        {
            _obj.transform.localPosition = Vector3.zero;
            _obj.transform.localRotation = Quaternion.identity;
        }
    }
}
