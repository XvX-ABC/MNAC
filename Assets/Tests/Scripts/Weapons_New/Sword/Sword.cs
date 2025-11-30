using System;
using System.Collections.Generic;
using System.Text;
using Tests.Utilities.Blackboards;
using UnityEditor;
using UnityEngine;

namespace Tests.Weapons_New.Sword
{
    internal class Sword : Weapon, ISword
    {
        [SerializeField]
        internal SwordTipTrigger tipTrigger;
        [SerializeField]
        internal GameObject ownerObj;
        [SerializeField]
        internal float length;
        [SerializeField]
        internal LayerMask collisionLayerMask;
        [SerializeField]
        internal LayerMask damageLayerMask;
        internal Vector3 worldUp = Vector3.up;

        internal Dictionary<SwordActionType, SwordAction> actions;
        internal SwordAction currentAction;
        internal List<SwordAction> currentEnabledActions;

        [SerializeField]
        SwordComponent[] _subComponents;
        Blackboard _blackboard;

        public Action<GameObject> HitAction { get => tipTrigger.EntryAction; set => tipTrigger.EntryAction = value; }
        public GameObject OwnerObj { get => ownerObj; set => ownerObj = value; }
        public Vector3 WorldUp { get => worldUp; set => worldUp = value; }

        public override WeaponType Type => WeaponType.Sword;

        public float Length { get => length; }
        public LayerMask CollisionLayerMask
        {
            get => collisionLayerMask;
            set => collisionLayerMask = value;
        }
        public LayerMask DamageLayerMask
        {
            get => damageLayerMask;
            set => damageLayerMask = value;
        }


        protected void Awake()
        {
            _blackboard = new();
            currentEnabledActions = new();
            InitializeActions();
        }
        public void Update()
        {
            var sb = new StringBuilder();
            foreach (var kv in actions)
            {
                sb.AppendLine($"{kv.Key}, {kv.Value}: {kv.Value.Enabled}");
            }
            //Debug.Log(sb.ToString());
        }
        protected virtual void InitializeActions()
        {
            actions = new();
            var arr = Enum.GetValues(typeof(SwordActionType));
            for (int i = 0; i < arr.Length; i++)
            {
                var v = arr.GetValue(i);
                var t = (SwordActionType)v;
                actions.Add(t, new SwordAction(this, t));
            }
        }
        protected virtual void Start()
        {
            _blackboard.TryRegisterField(SwordComponent.OwnerSword, this);
            foreach (var comp in _subComponents)
                comp?.Initialize(_blackboard);
        }
        protected virtual void OnDestroy()
        {
            foreach (var comp in _subComponents)
                comp?.Dispose();
            _blackboard.TryUnregisterField(SwordComponent.OwnerSword);
        }
        protected virtual void OnDrawGizmosSelected()
        {
            var pos = this.transform.position;
            var forward = this.transform.forward;
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(pos, forward * length);
        }
        public SwordAction GetSwordAction(SwordActionType type)
        {
            if (actions.TryGetValue(type, out var action))
                return action;
            return null;
        }
    }
}
