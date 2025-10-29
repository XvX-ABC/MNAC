using UnityEngine;
namespace Tests.Input
{
    public class VirtualInput : IVirtualInput
    {
        Vector3 _horizontalDirection;
        bool _isAscending;
        bool _isBoosting;
        bool _isQuickBoosting;
        public Vector3 HorizontalVector { get => _horizontalDirection; set => _horizontalDirection = value; }
        public bool Jump { get => _isAscending; set => _isAscending = value; }
        public bool Boost { get => _isBoosting; set => _isBoosting = value; }
        public bool QuickBoost { get => _isQuickBoosting; set => _isQuickBoosting = value; }

        bool IInput_Obsolete.Fire => throw new System.NotImplementedException();

        bool IInput_Obsolete.Reload => throw new System.NotImplementedException();

        bool IInput_Obsolete.Supply => throw new System.NotImplementedException();
    }
}