using UnityEngine;
namespace Tests.Input
{
    public interface IInput
    {
        public Vector3 HorizontalDirection { get; }
        public bool IsAscending { get; }
        public bool IsBoosting { get; }
        public bool Fire { get; }
        public bool Reload { get; }
        public bool Supply { get; }
    }
}