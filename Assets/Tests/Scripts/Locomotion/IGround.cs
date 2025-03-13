using UnityEngine;
namespace Tests.Locomotion
{

    public interface IGround
    {
        public GameObject Obj { get; }
        public Vector3 Normal { get; }
        public bool Touched { get; }
    }
}