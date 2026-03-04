using System;
using UnityEngine;

namespace MNAC.Interaction
{
    public interface IInteractable
    {
        public Guid ID { get; }
        public GameObject Obj { get; }
    }
    public interface IVolumetricInteractable : IInteractable
    {
        public Bounds Bounds { get; }
    }
}
