using BehaviorDesigner.Runtime.Tasks;
using System;
using UnityEngine;

namespace MNAC.AI
{
    [TaskCategory("Tests/AI")]
    internal abstract class AIConditionalBase : Conditional
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
