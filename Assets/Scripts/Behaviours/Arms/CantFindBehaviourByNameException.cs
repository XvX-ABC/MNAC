using System;

namespace MNAC.Behaviours.Arms
{
    internal class CantFindBehaviourByNameException : Exception
    {
        public CantFindBehaviourByNameException(string name) : base($"Can't find a behaviour by the name '{name}'")
        {

        }
    }
}
