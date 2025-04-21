using System;

namespace Tests.BodyBehaviour.Arm
{
    public class CantFindMountPointByNameException : Exception
    {
        public CantFindMountPointByNameException(string name) : base($"Can't find a mount point by the name '{name}'")
        {

        }
    }
}
