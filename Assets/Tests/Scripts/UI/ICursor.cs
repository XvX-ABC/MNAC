using UnityEngine;

namespace Tests.UI
{
    public interface ICursor
    {
        public Vector3 CursorPosition { get; set; }
        public bool HIde { get; set; }
    }
}
