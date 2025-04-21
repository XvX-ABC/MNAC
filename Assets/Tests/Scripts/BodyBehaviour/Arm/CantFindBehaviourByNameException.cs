using System;

namespace Tests.BodyBehaviour.Arm
{
    public class CantFindBehaviourByNameException : Exception
    {
        public CantFindBehaviourByNameException(string name) : base($"Can't find a mount point by the name '{name}'")
        {

        }
    }
}
