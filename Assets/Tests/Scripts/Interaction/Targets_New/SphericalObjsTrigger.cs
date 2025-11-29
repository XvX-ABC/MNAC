using Codice.CM.Triggers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tests.Interaction
{
    [RequireComponent(typeof(SphereCollider))]
    public class SphericalObjsTrigger : MonoBehaviour, ISphericalObjsTrigger
    {
        GameObjsTrigger _trigger;
        SphereCollider _collider;
        public float Radius
        {
            get => _collider.radius;
            set => _collider.radius = value;
        }
        public bool Enabled
        {
            get => enabled;
            set => enabled = _trigger.Enabled = value;
        }

        public IReadOnlyList<GameObject> CaughtItems => _trigger.CaughtItems;

        public Action<List<GameObject>> CaughtItemsChangedAction
        {
            get => _trigger.CaughtItemsChangedAction;
            set => _trigger.CaughtItemsChangedAction = value;
        }
        public Action<GameObject> ItemCaughtAction
        {
            get => _trigger.ItemCaughtAction;
            set => _trigger.ItemCaughtAction = value;
        }
        public Action<GameObject> ItemReleaseAction
        {
            get => _trigger.ItemReleaseAction;
            set => _trigger.ItemReleaseAction = value;
        }
        public LayerMask ExcludeLayerMask { get => _trigger.ExcludeLayerMask; set => _trigger.ExcludeLayerMask = value; }
        public LayerMask IncludeLayerMask { get => _trigger.IncludeLayerMask; set => _trigger.IncludeLayerMask = value; }

        public void AddItem(GameObject target)
        {
            _trigger.AddItem(target);
        }

        public void RemoveItem(GameObject target)
        {
            _trigger.RemoveItem(target);
        }
        void Awake()
        {
            _collider = GetComponent<SphereCollider>();
            _trigger = new GameObjsTrigger(_collider);
        }
        protected void OnTriggerEnter(Collider other)
        {
            _trigger.OnTriggerEnter(other);
        }
        protected void OnTriggerStay(Collider other)
        {
            _trigger.OnTriggerEnter(other);
        }
        protected void OnTriggerExit(Collider other)
        {
            _trigger.OnTriggerExit(other);
        }

        void ITargetsTrigger<GameObject>.OnTriggerEnter(Collider collider)
        {
        }

        void ITargetsTrigger<GameObject>.OnTriggerExit(Collider collider)
        {
        }
    }
}
