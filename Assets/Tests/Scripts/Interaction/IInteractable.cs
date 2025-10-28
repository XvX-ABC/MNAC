using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Tests.Interaction
{
    internal interface IInteractable : IGuidable
    {
        public GameObject Obj { get; }
    }
}
