using BehaviorDesigner.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests;

namespace Assets.Tests.Scripts.BDExtensions.Variables
{
    public class SharedTarget : SharedVariable<Target>
    {
        public static implicit operator SharedTarget(Target value)
        {
            return new SharedTarget { Value = value };
        }
    }
}
