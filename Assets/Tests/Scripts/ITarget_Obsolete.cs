using Locomotion;
using System;
using UnityEngine;

namespace Tests
{
    public interface ITarget_Obsolete
    {
        public GameObject Obj { get; set; }
        [Obsolete]
        public LocomotionContext Locomotion { get => default; set { } }
        public Vector3 Position { get; }
    }
}