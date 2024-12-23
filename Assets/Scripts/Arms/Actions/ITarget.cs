using UnityEngine;

namespace Assets.Scripts.Arms.Actions
{
    public interface ITarget
    {
        public Vector3 Position { get; set; }
    }
    internal interface IArmComponent
    {
        public ITarget Target { get; set; }
    }
    internal interface IArmLocomotionComponent : IArmComponent
    {
        public GameObject Body { get; set; }
    }
}
