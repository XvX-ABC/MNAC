using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Weapons.MultiMissileLauncher.Animation;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Tests.Scripts.AI
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
