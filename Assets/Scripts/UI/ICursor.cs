using UnityEngine;

namespace MNAC.UI
{
    public interface ICursor
    {
        public Vector3 CursorPosition { get; set; }
        public bool HIde { get; set; }
    }
}
