using TaskStatus = BehaviorDesigner.Runtime.Tasks.TaskStatus;

namespace MNAC.AI.BTExtensions
{
    internal class LauncherReserveAmmoEnough : LauncherConditionalBase
    {
        public override TaskStatus OnUpdate()
        {
            return launcher.ReserveAmmoAmount > 0 ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}
