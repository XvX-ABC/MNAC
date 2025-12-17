using System;
using UnityEngine;

namespace Tests.AI
{
    internal class Target : ITarget
    {
        GameObject _obj;
        public Target(GameObject obj)
        {
            _obj = obj ?? throw new ArgumentNullException(nameof(obj));
        }
        public GameObject Obj => _obj;

        public Vector3 Position => _obj.transform.position;
    }
}
