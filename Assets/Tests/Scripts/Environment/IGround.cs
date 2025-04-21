using UnityEngine;
namespace Tests.Environment
{

    public interface IGround
    {
        public Collision Collision { get; }
        public GameObject Obj { get; }
        public Vector3 Normal { get; }
        public bool Touched { get; }
    }
}