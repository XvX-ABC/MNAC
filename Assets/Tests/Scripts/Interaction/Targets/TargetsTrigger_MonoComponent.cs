using UnityEngine;

namespace Tests.Interaction.Targets
{
    [RequireComponent(typeof(Collider))]
    public class TargetsTrigger_MonoComponent : TargetsCatcherBase_MonoComponent
    {
        [SerializeField]
        LayerMask _mask;
        Collider _collider;
        public LayerMask ExcludeLayers
        {
            get => _mask;
            set
            {
                _collider.excludeLayers = value;
                _mask = value;
            }
        }
        public override bool Enabled
        {
            get => base.Enabled;
            set
            {
                base.Enabled = value;
                _collider.enabled = value;
            }
        }
        protected virtual void Awake()
        {
            _collider = GetCollider();
            ExcludeLayers = _mask;
        }
        protected virtual Collider GetCollider()
        {
            return GetComponent<Collider>();
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            var obj = other.gameObject;
            var target = GameObjTarget.GetInstance(obj);
            catcher.AddTarget(target);
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            var idx = catcher.targets.FindIndex(t => t is GameObjTarget target && target.Obj == other.gameObject);
            if (idx > -1)
            {
                var t = catcher.targets[idx];
                catcher.RemoveTarget(t);
                GameObjTarget.ReleaseInstance((GameObjTarget)t);
            }
        }
    }
}
