using UnityEngine;
namespace Tests.Input
{
    public class VirtualInput : IVirtualInput
    {
        Vector3 _horizontalDirection;
        bool _isAscending;
        bool _isBoosting;

        public Vector3 HorizontalDirection { get => _horizontalDirection; set => _horizontalDirection = value; }
        public bool IsAscending { get => _isAscending; set => _isAscending = value; }
        public bool IsBoosting { get => _isBoosting; set => _isBoosting = value; }

        bool IInput.Fire => throw new System.NotImplementedException();

        bool IInput.Reload => throw new System.NotImplementedException();

        bool IInput.Supply => throw new System.NotImplementedException();
    }
}