using System;
using UnityEngine;

namespace Tests.UI
{
    public interface IIndicatedTarget
    {
        public bool IsValid { get; }
        public Vector3 GetScreenPosition(Camera camera);
    }
}
