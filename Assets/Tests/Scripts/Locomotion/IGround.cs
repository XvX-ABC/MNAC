using UnityEngine;
namespace Tests.Locomotion
{

    public interface IGround
    {
        public Vector3 Normal { get; }
        public bool Touched { get; }
    }
}