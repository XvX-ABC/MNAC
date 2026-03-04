using System;
using UnityEngine;

namespace MNAC.AI
{
    internal class Target : ITarget
    {
        [SerializeField]
        GameObject _obj;
        private Target()
        {

        }
        public Target(GameObject obj)
        {
            _obj = obj ?? throw new ArgumentNullException(nameof(obj));
        }
        public GameObject Obj => _obj;

        public Vector3 Position => _obj.transform.position;
    }
}
