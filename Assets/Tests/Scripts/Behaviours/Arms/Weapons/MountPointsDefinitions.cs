using System;
using Tests.Characters.MountPoints;
using UnityEngine;

namespace Tests.Behaviours.Arms.Weapons
{
    [Serializable]
    public struct MountPointsDefinitions
    {
        [SerializeField]
        public MountPointLocation Launcher;
        [SerializeField]
        public MountPointLocation Sword;
    }
}
