using Locomotion;
using UnityEngine;

namespace Tests
{
    public interface ITarget
    {
        public GameObject Obj { get; set; }
        public LocomotionContext Locomotion { get; set; }
        public Vector3 Position { get; }
    }
}