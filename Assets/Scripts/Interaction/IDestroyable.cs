using System;
using UnityEngine;

namespace MNAC.Interaction
{
    public interface IDestroyable
    {
        public Action<GameObject> DestroyedCallback { get; set; }
        public void Destroy();
    }
}
