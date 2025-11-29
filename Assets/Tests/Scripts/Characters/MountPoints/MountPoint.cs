using System;
using UnityEngine;

namespace Tests.Characters.MountPoints
{
    [Serializable]
    internal class MountPoint : Utilities.MountPoints.MountPoint
    {
        [SerializeField]
        internal MountPointPlace place;

    }
}
