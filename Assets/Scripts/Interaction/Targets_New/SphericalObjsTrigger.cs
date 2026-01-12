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
        TeamMask _teamMask;
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
        public TeamMask TeamMask { get => _teamMask; set => _teamMask = value; }

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
        bool CheckTeamBy(GameObject obj)
        {
            if (obj.TryGetComponent<ITeamMember>(out var member))
                return member.CheckFriendlyBy(_teamMask);
            else
                return false;
        }
        protected virtual void OnTriggerEnter(Collider other)
        {
            var obj = other.gameObject;
            if (CheckTeamBy(obj))
                return;
            _trigger.OnTriggerEnter(other);
        }
        protected virtual void OnTriggerStay(Collider other)
        {
            if (CheckTeamBy(other.gameObject))
                return;
            _trigger.OnTriggerEnter(other);
        }
        protected virtual void OnTriggerExit(Collider other)
        {
            if (CheckTeamBy(other.gameObject))
                return;
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
