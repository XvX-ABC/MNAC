using UnityEngine;

namespace Assets.Scripts.Arms.Actions
{
    public interface IArmTarget
    {
        public Vector3 Position { get; set; }
    }
    internal interface IArmComponent
    {
        public IArmTarget Target { get; set; }
    }
    internal interface IArmLocomotionComponent : IArmComponent
    {
        public GameObject Body { get; set; }
    }
}
