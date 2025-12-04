using System;
using Tests.Utilities.Blackboards;
using Tests.Utilities.MountPoints;

namespace Tests.Characters.MountPoints
{
    internal static class BlackboardExtension_MountPoint
    {
        public static bool TryRegisterMountPointBlackboard(this Blackboard blackboard, MountPointBlackboard mountPointBlackboard)
        {
            if (blackboard == null)
                return false;
            return blackboard.TryRegisterField(MountPointFields.Blackboard_Main, mountPointBlackboard);
        }
        public static bool TryUnregisterMountPointBlackboard(this Blackboard blackboard)
        {
            if (blackboard == null)
                return false;
            return blackboard.TryUnregisterField(MountPointFields.Blackboard_Main);
        }
        public static bool TryGetMountPoint(this Blackboard blackboard, object key, out MountPoint mountPoint)
        {
            mountPoint = null;
            if (blackboard == null)
                return false;
            if (blackboard.TryReadValue<MountPointBlackboard>(MountPointFields.Blackboard_Main, out var subBlackboard))
                return subBlackboard.TryReadValue(key, out mountPoint);
            return false;
        }
        public static bool TryGetMountPoint(this Blackboard blackboard, MountPointLocation fieldEnum, out MountPoint mountPoint)
        {
            return blackboard.TryGetMountPoint(MountPointFields.GetGuid(fieldEnum), out mountPoint);
        }
        public static void TryGetMountPointOrThrowException(this Blackboard blackboard, object key, out MountPoint mountPoint)
        {
            if (!blackboard.TryGetMountPoint(key, out mountPoint))
                throw new Exception($"Key '{key}' not found in blackboard.");
        }
        public static void TryGetMountPointOrThrowException(this Blackboard blackboard, MountPointLocation fieldEnum, out MountPoint mountPoint)
        {
            blackboard.TryGetMountPointOrThrowException(MountPointFields.GetGuid(fieldEnum), out mountPoint);
        }
        public static bool TryRegisterMountPoint(this Blackboard blackboard, object key, MountPoint mountPoint)
        {
            if (blackboard == null)
                return false;
            if (blackboard.TryReadValue<MountPointBlackboard>(MountPointFields.Blackboard_Main, out var subBlackboard))
                return subBlackboard.TryRegisterField(key, mountPoint);
            return true;
        }
        public static bool TryRegisterMountPoint(this Blackboard blackboard, MountPointLocation fieldEnum, MountPoint mountPoint)
        {
            return blackboard.TryRegisterMountPoint(MountPointFields.GetGuid(fieldEnum), mountPoint);
        }
        public static void TryRegisterMountPointOrThrowException(this Blackboard blackboard, object key, MountPoint mountPoint)
        {
            if (!blackboard.TryRegisterMountPoint(key, mountPoint))
                throw new Exception("The mount point is not register correctly");
        }
        public static void TryRegisterMountPointOrThrowException(this Blackboard blackboard, MountPointLocation fieldEnum, MountPoint mountPoint)
        {
            if (!blackboard.TryRegisterMountPoint(fieldEnum, mountPoint))
                throw new Exception("The mount point is not register correctly");
        }
        public static bool TryRegisterMountPoint(this Blackboard blackboard, MountPoint mountPoint)
        {
            if (blackboard == null || mountPoint == null)
                return false;
            if (MountPointFields.TryFindGuid(mountPoint, out var key))
                return blackboard.TryRegisterMountPoint(key, mountPoint);
            return true;
        }
        public static void TryRegisterMountPointOrThrowException(this Blackboard blackboard, MountPoint mountPoint)
        {
            if (!blackboard.TryRegisterMountPoint(mountPoint))
                throw new Exception("The mount point is not register correctly");
        }
    }
}
