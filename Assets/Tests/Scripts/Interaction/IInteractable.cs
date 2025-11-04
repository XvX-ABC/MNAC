using System;
using UnityEngine;

namespace Tests.Interaction
{
    internal interface IInteractable
    {
        public Guid ID { get; }
        public GameObject Obj { get; }
    }
}
