using System;
using Tests.Characters.Interaction;
using Tests.Interaction;
using UnityEngine;

namespace Tess.AI
{
    internal class TargetLockerAdapter : TargetLockerBase
    {
        [SerializeField]
        AITargetLocker_Mono _lockerObj;
        internal AITargetLocker _targetLocker => _lockerObj.locker;
        public override Action<GameObject, GameObject> MainObjChangedAction { get => _targetLocker.MainObjChangedAction; set => _targetLocker.MainObjChangedAction = value; }
        public override ILockTarget MainLockTarget { get => _targetLocker.MainLockTarget; set => _targetLocker.MainLockTarget = value; }
        public override Action<ILockTarget, ILockTarget> MainTargetChangedAction { get => _targetLocker.MainTargetChangedAction; set => _targetLocker.MainTargetChangedAction = value; }
        public override ObstacleDetector ObstacleDetector { get => _targetLocker.ObstacleDetector; set => _targetLocker.ObstacleDetector = value; }
        public override float TargetChangeDuration { get => _targetLocker.TargetChangeDuration; set => _targetLocker.TargetChangeDuration = value; }

    }
}
