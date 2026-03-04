using System;
using UnityEngine;

namespace MNAC.UI
{
    public interface IIndicatedTarget
    {
        public bool IsValid { get; }
        public Vector3 GetScreenPosition(Camera camera);
    }
}
