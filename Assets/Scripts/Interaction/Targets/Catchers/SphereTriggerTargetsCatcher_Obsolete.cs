using System;
using UnityEngine;

namespace MNAC.Interaction.Targets
{
    [RequireComponent(typeof(SphereCollider))]
    [Obsolete]
    public class SphereTriggerTargetsCatcher_Obsolete : TargetsTrigger_MonoComponent, ISphereTriggerTargetsCatcher_Obsolete
    {
        SphereCollider _collider;
        public float Radius
        {
            get => _collider.radius;
            set => _collider.radius = Mathf.Max(0, value);
        }
        protected override Collider GetCollider()
        {
            _collider = GetComponent<SphereCollider>();
            _collider.isTrigger = true;
            _collider.layerOverridePriority = 10;
            return _collider;
        }
    }
}
