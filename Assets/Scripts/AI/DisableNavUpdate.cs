using UnityEngine;
using UnityEngine.AI;

namespace MNAC.AI
{
    internal class DisableNavUpdate : MonoBehaviour
    {
        NavMeshAgent _agent;
        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.updatePosition = false;
            _agent.updateRotation = false;
            Debug.Log($"up: {_agent.updatePosition}, ur: {_agent.updateRotation}");
        }
    }
}
