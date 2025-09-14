using BehaviorDesigner.Runtime;
using RootMotion.FinalIK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.BDExtensions.Variables
{
    public class SharedArmIK : SharedVariable<ArmIK>
    {
        public static implicit operator SharedArmIK(ArmIK value)
        {
            return new SharedArmIK { mValue = value };
        }
    }
}
