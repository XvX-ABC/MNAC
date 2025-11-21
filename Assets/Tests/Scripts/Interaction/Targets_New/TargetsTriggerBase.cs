using System;
using UnityEngine;

namespace Tests.Interaction
{
    public abstract class TargetsTriggerBase<T> : CatcherBase<T>
    {
        [SerializeField]
        Collider _collider;
        public LayerMask IncludeMask { get => _collider.includeLayers; set => _collider.includeLayers = value; }
        public LayerMask ExcludeMask { get => _collider.excludeLayers; set => _collider.excludeLayers = value; }
        public TargetsTriggerBase(Collider collider)
        {
            _collider = collider ?? throw new ArgumentNullException(nameof(collider));
        }
        public abstract void OnTriggerEnter(Collider collider);
        public abstract void OnTriggerExit(Collider collider);
    }
}
