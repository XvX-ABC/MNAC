using BehaviorDesigner.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.AI
{
    internal class SharedAICore : SharedVariable<AICore>
    {
        public static implicit operator SharedAICore(AICore core)
        {
            return new SharedAICore() { Value = core };
        }
    }
}
