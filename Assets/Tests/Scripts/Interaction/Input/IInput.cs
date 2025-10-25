using UnityEngine;
namespace Tests.Input
{
    public interface IInput
    {
        public Vector2 MousePosition { get => UnityEngine.Input.mousePosition; }
        public Vector3 HorizontalVector { get; }
        public bool Jump { get; }
        public bool QuickBoost { get; }
        public bool Boost { get; }
        public bool Fire { get; }
        public bool Reload { get; }
        public bool Supply { get; }
    }
}