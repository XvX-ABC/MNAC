
using UnityEngine;

namespace Tests.Interaction
{
    public interface ICursorReceiver
    {
        public bool Enabled { get; set; }
        public Vector3 CursorPosition { get; set; }
    }
}
