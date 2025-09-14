using System;
using UnityEngine;
namespace Tests.Environment
{
    internal class HitTarget : ITarget_Obsolete
    {
        internal RaycastHit hitInfo;
        public GameObject Obj { get => hitInfo.collider.gameObject; set => throw new NotImplementedException(); }
        public LocomotionContext Locomotion { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Vector3 Position => hitInfo.point;
    }
}