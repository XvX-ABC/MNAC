using System;
using UnityEngine;

namespace MNAC.Interaction
{
    public abstract class TargetsTriggerBase<T> : CatcherBase<T>, ITargetsTrigger<T>
    {
        [SerializeField]
        Collider _collider;
        public LayerMask IncludeLayerMask { get => _collider.includeLayers; set => _collider.includeLayers = value; }
        public LayerMask ExcludeLayerMask { get => _collider.excludeLayers; set => _collider.excludeLayers = value; }
        public TargetsTriggerBase(Collider collider)
        {
            _collider = collider ?? throw new ArgumentNullException(nameof(collider));
            _collider.isTrigger = true;
        }
        public abstract void OnTriggerEnter(Collider collider);

        public abstract void OnTriggerExit(Collider collider);
    }
}
