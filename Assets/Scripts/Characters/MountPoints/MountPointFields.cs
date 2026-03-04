using NUnit.Framework;
using System;
using UnityEngine;

namespace MNAC.Characters.MountPoints
{
    public enum MountPointLocation
    {
        None,
        Blackboard_Main,
        Left_Hand_Weapon,
        Left_LowerArm_Weapon,
        Right_Hand_Weapon,
        Right_LowerArm_Weapon,
        Right_Chest_Trigger,
        Left_Chest_Trigger
    }
    internal static class MountPointFields
    {

        static MountPointFields()
        {
            Blackboard_Main = Guid.NewGuid();
            Left_Hand_Weapon = Guid.NewGuid();
            Left_LowerArm_Weapon = Guid.NewGuid();
            Left_Chest_Trigger = Guid.NewGuid();
            Right_Hand_Weapon = Guid.NewGuid();
            Right_LowerArm_Weapon = Guid.NewGuid();
            Right_Chest_Trigger = Guid.NewGuid();
        }
        public static readonly Guid Blackboard_Main;
        public static readonly Guid Right_Hand_Weapon;
        private static readonly Guid Right_LowerArm_Weapon;
        public static readonly Guid Right_Chest_Trigger;
        public static readonly Guid Left_Hand_Weapon;
        private static readonly Guid Left_LowerArm_Weapon;
        public static readonly Guid Left_Chest_Trigger;
        public static MountPointLocation GetEnumType(Guid guid)
        {
            if (guid == Guid.Empty)
                return MountPointLocation.None;
            if (guid == Blackboard_Main)
                return MountPointLocation.Blackboard_Main;
            else if (guid == Left_Hand_Weapon)
                return MountPointLocation.Left_Hand_Weapon;
            else if (guid == Right_Hand_Weapon)
                return MountPointLocation.Right_Hand_Weapon;
            else if (guid == Left_LowerArm_Weapon)
                return MountPointLocation.Left_LowerArm_Weapon;
            else if (guid == Right_LowerArm_Weapon)
                return MountPointLocation.Right_LowerArm_Weapon;
            else if (guid == Right_Chest_Trigger)
                return MountPointLocation.Right_Chest_Trigger;
            else if (guid == Left_Chest_Trigger)
                return MountPointLocation.Left_Chest_Trigger;
            else
                return MountPointLocation.None;
        }
        public static Guid GetGuid(MountPointLocation enumType)
        {
            return enumType switch
            {
                MountPointLocation.Blackboard_Main => Blackboard_Main,
                MountPointLocation.Left_Hand_Weapon => Left_Hand_Weapon,
                MountPointLocation.Left_LowerArm_Weapon => Left_LowerArm_Weapon,
                MountPointLocation.Right_Hand_Weapon => Right_Hand_Weapon,
                MountPointLocation.Right_LowerArm_Weapon => Right_LowerArm_Weapon,
                MountPointLocation.Right_Chest_Trigger => Right_Chest_Trigger,
                MountPointLocation.Left_Chest_Trigger => Left_Chest_Trigger,
                _ => Guid.Empty,
            };
        }
        public static bool TryFindGuid(MountPoint mountPoint, out Guid guid)
        {
            guid = GetGuid(mountPoint.place);
            if (guid == Guid.Empty)
            {
                Debug.LogWarning($"Cannot find a field to match the mount point '{mountPoint.Name}''");
                return false;
            }
            return true;
        }
    }
}
