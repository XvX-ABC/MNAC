using System;
using UnityEngine;

namespace MNAC.Characters.MountPoints
{
    [Serializable]
    internal class MountPoint : MNAC.Utilities.MountPoints.MountPoint
    {
        [SerializeField]
        internal MountPointLocation place;

    }
}
