using BehaviorDesigner.Runtime.Tasks;
using System;
using Unity.VisualScripting;
using UnityEngine;
using Action = BehaviorDesigner.Runtime.Tasks.Action;

namespace MNAC.AI
{
    [TaskCategory("Tests/AI")]
    internal abstract class AIActionBase : Action
    {
        [SerializeField]
        SharedAICore _sharedCore;
        protected AICore core;
        public override void OnAwake()
        {
            base.OnAwake();
            core = _sharedCore.Value ?? throw new ArgumentNullException(nameof(_sharedCore.Value));
        }
    }
}
