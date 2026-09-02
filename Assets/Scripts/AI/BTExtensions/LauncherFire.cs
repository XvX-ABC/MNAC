using TaskStatus = BehaviorDesigner.Runtime.Tasks.TaskStatus;

namespace MNAC.AI.BTExtensions
{
    internal class LauncherFire : LauncherActionBase
    {
        public override void OnStart()
        {
            base.OnStart();
            weaponInput.fire = true;
        }
        public override TaskStatus OnUpdate()
        {
            return launcher.MagazineAmmoAmount > 0 ? TaskStatus.Running : TaskStatus.Failure;
        }
        public override void OnEnd()
        {
            weaponInput.fire = false;
            base.OnEnd();
        }
    }
}
