using UnityEngine;

namespace Tests.Interaction.Targets
{
    [RequireComponent(typeof(SphereCollider))]
    public class SphereTriggerTargetsCatcher : TargetsTrigger_MonoComponent, ISphereTriggerTargetsCatcher
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
