using UnityEngine;
namespace Tests.Locomotion
{
    public interface IInput
    {
        public Vector3 HorizontalDirection { get; }
        public bool IsAscending { get; }
        public bool IsBoosting { get; }
    }
}