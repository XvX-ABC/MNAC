using System;
using Tests.Interaction;
using UnityEngine;

namespace Tests.Weapons_New.Sword
{
    [RequireComponent(typeof(Collider))]
    internal class SwordTipTrigger : SwordComponent
    {
        GameObjsTrigger _trigger;
        Collider _collider;

        public LayerMask ExcludedLayerMask { get => _trigger.ExcludeLayerMask; set => _trigger.ExcludeLayerMask = value; }
        public LayerMask IncludedLayerMask { get => _trigger.IncludeLayerMask; set => _trigger.IncludeLayerMask = value; }
        public Action<GameObject> EntryAction { get => _trigger.ItemCaughtAction; set => _trigger.ItemCaughtAction = value; }
        public Action<GameObject> ExitAction { get => _trigger.ItemReleaseAction; set => _trigger.ItemReleaseAction = value; }
        public void AddItem(GameObject target)
        {
            _trigger.AddItem(target);
        }

        public void RemoveItem(GameObject target)
        {
            _trigger.RemoveItem(target);
        }

        protected override void Awake()
        {
            base.Awake();
            _collider = GetComponent<Collider>();
            _collider.isTrigger = true;
            _trigger = new(_collider);
        }
        protected virtual void OnEnable()
        {
            _trigger.Enabled = true;
            _collider.enabled = true;
        }
        protected virtual void OnDisable()
        {
            _trigger.Enabled = false;
            _collider.enabled = false;
        }
        void OnTriggerEnter(Collider other)
        {
            _trigger.OnTriggerEnter(other);
        }
        private void OnTriggerExit(Collider other)
        {
            _trigger.OnTriggerExit(other);
        }
    }
}
