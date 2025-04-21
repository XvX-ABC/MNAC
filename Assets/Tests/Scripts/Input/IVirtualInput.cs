using UnityEngine;
namespace Tests.Input
{
    public interface IVirtualInput : IInput
    {
        public new Vector3 HorizontalDirection { get; set; }
        public new bool IsAscending { get; set; }
        public new bool IsBoosting { get; set; }
    }
}