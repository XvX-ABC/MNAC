using System;
using UnityEngine;
namespace Tests.Input
{
    [Obsolete]
    public interface IInput_Obsolete
    {
        public Vector2 MousePosition { get => UnityEngine.Input.mousePosition; }
        public Vector3 HorizontalVector { get; }
        public bool Jump { get; }
        public bool QuickBoost { get; }
        public bool Boost { get; }
        [Obsolete]
        public bool Fire { get; }
        [Obsolete]
        public bool Reload { get; }
        [Obsolete]
        public bool Supply { get; }

    }
}