using System;
using System.Collections.Generic;
using Tests.Utilities.Blackboards;
using UnityEngine;

namespace Tests.Weapons_New.Sword
{
    internal class Sword : MonoBehaviour, ISword
    {
        [SerializeField]
        internal SwordTipTrigger tipTrigger;
        [SerializeField]
        internal GameObject ownerObj;
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

        public string Name => this.name;

        public WeaponType Type => WeaponType.Sword;

        public GameObject Obj => this.gameObject;

        protected void Awake()
        {
            _blackboard = new();
            currentEnabledActions = new();
            InitializeActions();
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
        private void Start()
        {
            _blackboard.TryRegisterField(SwordComponent.OwnerSword, this);
            foreach (var comp in _subComponents)
                comp?.Initialize(_blackboard);
        }
        protected virtual void OnDisable()
        {
            foreach (var comp in _subComponents)
                comp?.Dispose();
            _blackboard.TryUnregisterField(SwordComponent.OwnerSword);
        }
        public SwordAction GetSwordAction(SwordActionType type)
        {
            if (actions.TryGetValue(type, out var action))
                return action;
            return null;
        }
    }
}
