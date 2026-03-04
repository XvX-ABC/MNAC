using System;
using MNAC.Characters.MountPoints;
using UnityEngine;

namespace MNAC.Behaviours.Arms.Weapons
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
