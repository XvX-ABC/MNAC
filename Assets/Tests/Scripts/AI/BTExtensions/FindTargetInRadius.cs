using BehaviorDesigner.Runtime.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Action = BehaviorDesigner.Runtime.Tasks.Action;
using TaskStatus = BehaviorDesigner.Runtime.Tasks.TaskStatus;

namespace Tests.AI
{
    [TaskCategory("Tests/AI")]
    internal class FindTargetInRadius : Action
    {
        [SerializeField]
        SharedAICore _sharedAICore;
        AICore _core;
        public override void OnAwake()
        {
            _core = _sharedAICore.Value;
        }
        public override TaskStatus OnUpdate()
        {
            return base.OnUpdate();
        }
    }
}
