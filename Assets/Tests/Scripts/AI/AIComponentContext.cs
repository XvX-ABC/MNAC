using Tests.Utilities.Blackboards;
using UnityEngine.AI;

namespace Tests.AI
{
    internal class AIComponentContext
    {
        AICore _core;
        NavMeshAgent _navAgent;
        Blackboard _characterBlackboard;
        AINavigation _navigation;

        internal NavMeshAgent navAgent { get => _navAgent; set => _navAgent = value; }
        internal Blackboard characterBlackboard { get => _characterBlackboard; set => _characterBlackboard = value; }
        internal AICore core { get => _core; set => _core = value; }
        internal ITarget target { get => _core.interactableTarget; set => _core.interactableTarget = value; }
        internal AINavigation navigation { get => _navigation; set => _navigation = value; }
    }
}
