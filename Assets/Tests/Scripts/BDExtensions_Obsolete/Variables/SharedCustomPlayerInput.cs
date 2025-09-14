using BehaviorDesigner.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Input;

namespace Assets.Tests.Scripts.BDExtensions.Variables
{
    [Serializable]
    public class SharedCustomInputTypes : SharedVariable<InputTypes>
    {
        public static implicit operator SharedCustomInputTypes(InputTypes value)
        {
            return new SharedCustomInputTypes { Value = value };
        }
    }
    [Serializable]
    public class SharedCustomPlayerInput : SharedVariable<CustomPlayerInput>
    {
        public static implicit operator SharedCustomPlayerInput(CustomPlayerInput value)
        {
            return new SharedCustomPlayerInput { Value = value };
        }
    }
    [Serializable]
    public class SharedHybridInput : SharedVariable<HybridInput>
    {
        public static implicit operator SharedHybridInput(HybridInput value)
        {
            return new SharedHybridInput { Value = value };
        }
    }
}
