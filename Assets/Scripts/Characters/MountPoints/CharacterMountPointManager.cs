using System;
using System.Collections.Generic;
using MNAC.Utilities.Blackboards;
using MNAC.Utilities.Composable;
using UnityEngine;

namespace MNAC.Characters.MountPoints
{
    [Serializable]
    internal class CharacterMountPointManager : ComponentBase
    {
        [SerializeField]
        List<MountPoint> _mountPoints;
        MountPointBlackboard _mountPointBlabkboard;
        public CharacterMountPointManager()
        {
            _mountPoints = new List<MountPoint>();
        }
        public override string Name => "mount_point_manager";

        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            _mountPointBlabkboard = new MountPointBlackboard();
            blackboard.TryRegisterMountPointBlackboard(_mountPointBlabkboard);
            for (int i = 0; i < _mountPoints.Count; i++)
            {
                var m = _mountPoints[i];
                if (!MountPointFields.TryFindGuid(m, out var guid))
                    continue;
                blackboard.TryRegisterMountPoint(guid, m);
            }
            _mountPointBlabkboard.TryReadValueOrThrowException<FieldChangeHandler>(MiddlewareFields.FieldChangeHandler, out var handler);
            handler.RegisterGlobalAction(WhenMountPointChanged);
        }
        void WhenMountPointChanged(FieldEventType type, object oldValue, object newValue)
        {
            if (type == FieldEventType.Register)
            {
                if (newValue is MountPoint mountPoint && MountPointFields.TryFindGuid(mountPoint, out _))
                    _mountPoints.Add(mountPoint);
            }
            else if (type == FieldEventType.Unregister && oldValue is MountPoint m)
            {
                _mountPoints.Remove(m);
            }
            else if (type == FieldEventType.Writing)
            {
                if (oldValue is MountPoint om && MountPointFields.TryFindGuid(om, out _))
                    _mountPoints.Remove(om);
                if (newValue is MountPoint nm && MountPointFields.TryFindGuid(nm, out _))
                    _mountPoints.Add(nm);
            }
        }
        public override void Dispose()
        {
            base.Dispose();
            blackboard.TryUnregisterMountPointBlackboard();

        }
    }
}
