using BehaviorDesigner.Runtime;
using System;
using System.Collections.Generic;
using MNAC.Characters;
using MNAC.Interaction;
using MNAC.Utilities.Blackboards;
using UnityEngine;
using UnityEngine.AI;

namespace MNAC.AI
{
    [DefaultExecutionOrder(0)]
    internal class AICore : MonoBehaviour
    {
        [SerializeField]
        NavMeshAgent _navAgent;
        [SerializeField]
        GameObject _targetObj;
        [SerializeField]
        IDamageable _damageable;
        [SerializeField]
        BehaviorTree _bt;
        [SerializeField]
        AIComponent_Mono[] _components;
        List<AIComponent> _internalComponents;


        ITarget _interactableTarget;
        Blackboard _blackboard;
        AIComponentContext _componentContext;

        [SerializeField]
        CharacterBase _character;
        Blackboard _characterBlackboard;


        Action<ITarget, ITarget> _targetChangedAction;

        internal ITarget interactableTarget
        {
            get => _interactableTarget;
            set
            {
                if (_interactableTarget != value)
                {
                    _targetChangedAction?.Invoke(_interactableTarget, value);
                }
                _interactableTarget = value;
            }
        }
        internal NavMeshAgent navAgent { get => _navAgent; set => _navAgent = value; }
        internal Action<ITarget, ITarget> targetChangedAction { get => _targetChangedAction; set => _targetChangedAction = value; }
        internal Blackboard characterBlackboard { get => _characterBlackboard; set => _characterBlackboard = value; }
        internal AIComponentContext componentContext { get => _componentContext; set => _componentContext = value; }
        public GameObject TargetObj
        {
            get => _targetObj;
            set
            {
                if (_targetObj != null)
                {
                    _damageable = _targetObj.GetComponent<IDamageable>();
                    interactableTarget = new Target(_targetObj);
                }
                _targetObj = value;
            }
        }

        public BehaviorTree BehaviourTree { get => _bt; set => _bt = value; }

        private void Awake()
        {
            _blackboard = new();
            //_components = GetComponentsInChildren<AIComponent_Mono>();

            _internalComponents = new();

            TargetObj = _targetObj;

            _componentContext = new()
            {
                navAgent = _navAgent,
                core = this,
            };

            _bt = GetComponent<BehaviorTree>();
        }
        private void Start()
        {
            _characterBlackboard = _character.blackboard ?? throw new NullReferenceException("The character blackboard can't is null.");

            InitializeComponents(_componentContext);
            InitializeInternalComponents(_componentContext);
            _bt.enabled = true;
        }
        private void Update()
        {
#if UNITY_EDITOR
            if (_targetObj != null && interactableTarget == null)
                interactableTarget = new Target(_targetObj);
            else if (_targetObj == null && interactableTarget != null)
                interactableTarget = null;
#endif
            if (_damageable != null && !_damageable.HP.IsAlive)
                interactableTarget = null;
        }
        private void OnDestroy()
        {
            ComponentDispose();
            InternalComponentDispose();
        }
        internal void RegisterInternalComponent(AIComponent comp)
        {
            if (_internalComponents.Contains(comp))
                return;
            if (didStart)
                comp.Initialize(_componentContext);
            _internalComponents.Add(comp);
        }
        internal void UnregisterInternalComponent(AIComponent comp)
        {
            var idx = _internalComponents.FindIndex(c => c == comp);
            if (idx == -1)
                return;
            comp.Dispose();
            _internalComponents.RemoveAt(idx);
        }
        void InitializeComponents(AIComponentContext context)
        {
            foreach (var comp in _components)
            {
                comp.Initialize(context);
            }

        }
        void InitializeInternalComponents(AIComponentContext context)
        {
            foreach (var comp in _internalComponents)
            {
                comp.Initialize(context);
            }

        }
        void ComponentDispose()
        {
            foreach (var comp in _components)
            {
                comp.Dispose();
            }
        }
        void InternalComponentDispose()
        {
            foreach (var comp in _internalComponents)
            {
                comp.Dispose();
            }
        }

    }
}
