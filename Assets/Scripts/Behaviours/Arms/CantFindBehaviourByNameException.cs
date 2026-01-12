using System;

namespace Tests.Behaviours.Arms
{
    internal class CantFindBehaviourByNameException : Exception
    {
        public CantFindBehaviourByNameException(string name) : base($"Can't find a behaviour by the name '{name}'")
        {

        }
    }
}
