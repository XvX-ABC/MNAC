using System;
using Tests.Behaviours;
using Tests.Interaction;
using Tests.Utilities.Blackboards;
using UnityEngine;

namespace Tests.Characters
{
    internal abstract class TargetLocker : CharacterComponent, ITargetLocker
    {
        public Action<GameObject, GameObject> MainObjChangedAction { get => locker.MainObjChangedAction; set => locker.MainObjChangedAction = value; }
        public ILockTarget MainLockTarget { get => locker.MainLockTarget; set => locker.MainLockTarget = value; }
        public Action<ILockTarget, ILockTarget> MainTargetChangedAction { get => locker.MainTargetChangedAction; set => locker.MainTargetChangedAction = value; }
        public ObstacleDetector ObstacleDetector { get => locker.ObstacleDetector; set => locker.ObstacleDetector = value; }
        protected abstract Tests.Behaviours.TargetLocker locker { get; }
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            blackboard.TryRegisterField(CharacterBlackboardFields.Character_Component_TargetLocker, this);
        }
        public override void Dispose()
        {
            blackboard.TryUnregisterField(CharacterBlackboardFields.Character_Component_TargetLocker);
            base.Dispose();
        }
        public void OnFixedUpdate()
        {
            locker.OnFixedUpdate();
        }

        public void OnLateUpdate()
        {
            locker.OnLateUpdate();
        }
    }

}
