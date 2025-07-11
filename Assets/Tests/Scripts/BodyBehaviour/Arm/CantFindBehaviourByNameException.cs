using System;

namespace Tests.Behaviours.Arm
{
    public class CantFindBehaviourByNameException : Exception
    {
        public CantFindBehaviourByNameException(string name) : base($"Can't find a behaviour by the name '{name}'")
        {

        }
    }
}
