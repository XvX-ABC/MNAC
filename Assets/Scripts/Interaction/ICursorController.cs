
using UnityEngine;

namespace MNAC.Interaction
{
    public interface ICursorController
    {
        public bool Enabled { get; set; }
        public Vector3 CursorPosition { get; set; }
    }
}
