using UnityEngine;

namespace Tests.UI
{
    public interface ICursor
    {
        public Vector3 MousePosition { get; set; }
        public bool HIde { get; set; }
    }
}
