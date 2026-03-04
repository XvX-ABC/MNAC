using System;
using MNAC.Behaviours;
using MNAC.Interaction;
using MNAC.Utilities.Blackboards;
using UnityEngine;

namespace MNAC.Characters.Interaction
{
    internal abstract class TargetLockerBase : CharacterComponent, ITargetLocker
    {
        public abstract Action<GameObject, GameObject> MainObjChangedAction { get; set; }
        public abstract ILockTarget MainLockTarget { get; set; }
        public abstract Action<ILockTarget, ILockTarget> MainTargetChangedAction { get; set; }
        public abstract ObstacleDetector ObstacleDetector { get; set; }
        public abstract float TargetChangeDuration { get; set; }

        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            blackboard.TryRegisterField(CharacterBlackboardFields.Character_Components_TargetLocker, this);
        }
        public override void Dispose()
        {
            blackboard.TryUnregisterField(CharacterBlackboardFields.Character_Components_TargetLocker);
            base.Dispose();
        }
        public virtual void OnFixedUpdate()
        {
        }

        public virtual void OnLateUpdate()
        {
        }
    }

}
