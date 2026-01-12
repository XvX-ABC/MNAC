using System;
using UnityEngine;

namespace Tests.Interaction
{
    public interface IDestroyable
    {
        public Action<GameObject> DestroyedCallback { get; set; }
        public void Destroy();
    }
}
