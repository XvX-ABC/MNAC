using NUnit.Framework;
using System;
using UnityEngine;

namespace Tests.Characters.MountPoints
{
    public enum MountPointPlace
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
        public static MountPointPlace GetEnumType(Guid guid)
        {
            if (guid == Guid.Empty)
                return MountPointPlace.None;
            if (guid == Blackboard_Main)
                return MountPointPlace.Blackboard_Main;
            else if (guid == Left_Hand_Weapon)
                return MountPointPlace.Left_Hand_Weapon;
            else if (guid == Right_Hand_Weapon)
                return MountPointPlace.Right_Hand_Weapon;
            else if (guid == Left_LowerArm_Weapon)
                return MountPointPlace.Left_LowerArm_Weapon;
            else if (guid == Right_LowerArm_Weapon)
                return MountPointPlace.Right_LowerArm_Weapon;
            else if (guid == Right_Chest_Trigger)
                return MountPointPlace.Right_Chest_Trigger;
            else if (guid == Left_Chest_Trigger)
                return MountPointPlace.Left_Chest_Trigger;
            else
                return MountPointPlace.None;
        }
        public static Guid GetGuid(MountPointPlace enumType)
        {
            return enumType switch
            {
                MountPointPlace.Blackboard_Main => Blackboard_Main,
                MountPointPlace.Left_Hand_Weapon => Left_Hand_Weapon,
                MountPointPlace.Left_LowerArm_Weapon => Left_LowerArm_Weapon,
                MountPointPlace.Right_Hand_Weapon => Right_Hand_Weapon,
                MountPointPlace.Right_LowerArm_Weapon => Right_LowerArm_Weapon,
                MountPointPlace.Right_Chest_Trigger => Right_Chest_Trigger,
                MountPointPlace.Left_Chest_Trigger => Left_Chest_Trigger,
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
