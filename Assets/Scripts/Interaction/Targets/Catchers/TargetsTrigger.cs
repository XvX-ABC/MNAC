using System;
using UnityEngine;

namespace MNAC.Interaction.Targets
{
    public class TargetsTrigger : TargetsCatcherBase
    {
        protected Collider collider;
        public LayerMask ExcludeLayers
        {
            get => collider.excludeLayers;
            set => collider.excludeLayers = value;
        }
        public TargetsTrigger(Collider collider)
        {
            this.collider = collider ?? throw new ArgumentNullException(nameof(collider));
            collider.isTrigger = true;
        }
        public virtual void OnTriggerEnter(Collider collider)
        {
            var obj = collider.gameObject;
            var target = GameObjTarget.GetInstance(obj);
            AddTarget(target);
        }
        public virtual void OnTriggerExit(Collider collider)
        {
            var obj = collider.gameObject;
            var idx = targets.FindIndex(t => ((GameObjTarget)t).obj == obj);
            if (idx > -1)
                RemoveTarget(targets[idx]);
        }
    }
}
