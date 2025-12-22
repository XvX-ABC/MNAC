using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace Tests.AI
{
    internal class StopMoveToTarget : AIActionBase
    {
        AINavigation _navigation;
        public override void OnStart()
        {
            base.OnStart();
            _navigation = core.componentContext.navigation;
            _navigation.Destination = null;
        }
        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Success;
        }
    }
    internal class MoveToTarget : AIActionBase
    {
        AINavigation _navgation;
        public override void OnStart()
        {
            base.OnStart();
            _navgation = core.componentContext.navigation;
            _navgation.Destination = core.componentContext.target;
            Debug.Log("move to target");
        }
        public override TaskStatus OnUpdate()
        {
            if (!_navgation.Enabled)
                return TaskStatus.Failure;
            return _navgation.IsMoving ? TaskStatus.Running : TaskStatus.Success;
        }
    }
}
