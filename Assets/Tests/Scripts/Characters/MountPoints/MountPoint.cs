using System;
using UnityEngine;

namespace Tests.Characters.MountPoints
{
    [Serializable]
    internal class MountPoint : Utilities.MountPoints.MountPoint
    {
        [SerializeField]
        internal MountPointLocation place;

    }
}
