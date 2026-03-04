using System;

namespace MNAC.Behaviours.Arms
{
    internal class CantFindMountPointByNameException : Exception
    {
        public CantFindMountPointByNameException(string name) : base($"Can't find a mount point by the name '{name}'")
        {

        }
    }
}
