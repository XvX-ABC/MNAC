using BehaviorDesigner.Runtime.Tasks;
using Tess.AI;
using UnityEngine;
using Random = Unity.Mathematics.Random;
namespace Tests.AI
{
    internal class StartFindTarget : AIActionBase
    {
        AITargetLocker _locker;
        public override void OnStart()
        {
            base.OnStart();
            _locker = core.componentContext.targetLocker;
            _locker.Enabled = true;
            Debug.Log("start find target");
        }
        public override TaskStatus OnUpdate()
        {
            return _locker.Enabled ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
    internal class StopFindTarget : AIActionBase
    {
        AITargetLocker _locker;
        public override void OnStart()
        {
            base.OnStart();
            _locker = core.componentContext.targetLocker;
            _locker.Enabled = false;
            Debug.Log("stop find target");
        }
        public override TaskStatus OnUpdate()
        {
            return _locker.Enabled ? TaskStatus.Failure : TaskStatus.Success;
        }
    }
    internal class LockedTarget : AIConditionalBase
    {
        AITargetLocker _locker;
        public override void OnStart()
        {
            base.OnStart();
            _locker = core.componentContext.targetLocker;
        }
        public override TaskStatus OnUpdate()
        {
            return _locker.MainLockTarget == null ? TaskStatus.Failure : TaskStatus.Success;
        }
    }
}
