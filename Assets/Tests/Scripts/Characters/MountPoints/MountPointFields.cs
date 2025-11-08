using NUnit.Framework;
using System;
using UnityEngine;

namespace Tests.Characters.MountPoints
{
    internal static class MountPointFields
    {
        public enum Enum
        {
            None,
            Blackboard_Main,
            Left_Arm_Hand_Weapon,
            Right_Arm_Hand_Weapon,
            Right_Chest_Trigger,
            Left_Chest_Trigger
        }
        static MountPointFields()
        {
            Blackboard_Main = Guid.NewGuid();
            Left_Arm_Hand_Weapon = Guid.NewGuid();
            Left_Chest_Trigger = Guid.NewGuid();
            Right_Arm_Hand_Weapon = Guid.NewGuid();
            Right_Chest_Trigger = Guid.NewGuid();
        }
        public static readonly Guid Blackboard_Main;
        public static readonly Guid Right_Arm_Hand_Weapon;
        public static readonly Guid Right_Chest_Trigger;
        public static readonly Guid Left_Arm_Hand_Weapon;
        public static readonly Guid Left_Chest_Trigger;
        public static Enum GetEnumType(Guid guid)
        {
            if (guid == Guid.Empty)
                return Enum.None;
            if (guid == Blackboard_Main)
                return Enum.Blackboard_Main;
            else if (guid == Left_Arm_Hand_Weapon)
                return Enum.Left_Arm_Hand_Weapon;
            else if (guid == Right_Arm_Hand_Weapon)
                return Enum.Right_Arm_Hand_Weapon;
            else if (guid == Right_Chest_Trigger)
                return Enum.Right_Chest_Trigger;
            else if (guid == Left_Chest_Trigger)
                return Enum.Left_Chest_Trigger;
            else
                return Enum.None;
        }
        public static Guid GetGuid(Enum enumType)
        {
            return enumType switch
            {
                Enum.Blackboard_Main => Blackboard_Main,
                Enum.Left_Arm_Hand_Weapon => Left_Arm_Hand_Weapon,
                Enum.Right_Arm_Hand_Weapon => Right_Arm_Hand_Weapon,
                Enum.Right_Chest_Trigger => Right_Chest_Trigger,
                Enum.Left_Chest_Trigger => Left_Chest_Trigger,
                _ => Guid.Empty,
            };
        }
        public static bool TryFindGuid(MountPoint mountPoint, out Guid guid)
        {
            guid = GetGuid(mountPoint.field);
            if (guid == Guid.Empty)
            {
                Debug.LogWarning($"Cannot find a field to match the mount point '{mountPoint.Name}''");
                return false;
            }
            return true;
        }
    }
}
