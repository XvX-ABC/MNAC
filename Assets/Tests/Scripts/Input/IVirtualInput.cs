using UnityEngine;
namespace Tests.Input
{
    public interface IVirtualInput : IInput
    {
        public new Vector3 HorizontalVector { get; set; }
        public new bool Jump { get; set; }
        public new bool Boost { get; set; }
    }
}