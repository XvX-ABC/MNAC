using UnityEngine;
namespace Tests.Input
{
    public interface IVirtualInput : IInput_Obsolete
    {
        public new Vector3 HorizontalVector { get; set; }
        public new bool Jump { get; set; }
        public new bool Boost { get; set; }
    }
}