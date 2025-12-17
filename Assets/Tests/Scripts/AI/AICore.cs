using System;
using Tests.Characters;
using Tests.Utilities.Blackboards;
using UnityEngine;
using UnityEngine.AI;

namespace Tests.AI
{
    internal class AICore : MonoBehaviour
    {
        [SerializeField]
        NavMeshAgent _navAgent;
        [SerializeField]
        GameObject _targetObj;
        AIComponent[] _components;


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
                    _interactableTarget = value;
                }
            }
        }
        internal NavMeshAgent navAgent { get => _navAgent; set => _navAgent = value; }
        internal Action<ITarget, ITarget> targetChangedAction { get => _targetChangedAction; set => _targetChangedAction = value; }
        internal Blackboard characterBlackboard { get => _characterBlackboard; set => _characterBlackboard = value; }

        private void Awake()
        {
            _blackboard = new();
            _components = GetComponentsInChildren<AIComponent>();
            _interactableTarget = new Target(_targetObj);

        }
        private void Start()
        {
            _characterBlackboard = _character.blackboard;
            _componentContext = new()
            {
                navAgent = _navAgent,
                core = this,
                characterBlackboard = _characterBlackboard,
            };
            InitializeComponents(_componentContext);
        }
        private void OnDestroy()
        {
            ComponentDispose();
        }
        void InitializeComponents(AIComponentContext context)
        {
            foreach (var comp in _components)
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

    }
}
