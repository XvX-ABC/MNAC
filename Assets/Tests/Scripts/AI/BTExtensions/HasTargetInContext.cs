using TaskStatus = BehaviorDesigner.Runtime.Tasks.TaskStatus;

namespace Tests.AI
{
    internal class HasTargetInContext : AIConditionalBase
    {
        public override TaskStatus OnUpdate()
        {
            return core.componentContext.target != null ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}
