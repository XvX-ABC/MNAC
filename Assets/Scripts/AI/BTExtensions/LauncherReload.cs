using Tests.Utilities.Timeline;
using Tests.Weapons_New.Launcher;
using TaskStatus = BehaviorDesigner.Runtime.Tasks.TaskStatus;

namespace Assets.Tests.Scripts.AI.BTExtensions
{
    internal class LauncherReload : LauncherActionBase
    {
        ITimeline _reloadTimeline;
        bool _reloadSucceed;
        bool _reloadExecuted;
        public override void OnStart()
        {
            base.OnStart();
            _reloadTimeline = launcher.ReloadTimeline;
            _reloadTimeline.EndAction += ReloadEnded;
            launcher.ReloadCallback += UpdateReloadSucceed;
            weaponInput.reload = true;
        }
        void ReloadEnded(TimelineContext _)
        {
            _reloadExecuted = true;
        }
        void UpdateReloadSucceed(ILauncher _)
        {
            _reloadSucceed = true;
        }
        public override TaskStatus OnUpdate()
        {
            if (!_reloadExecuted)
                return TaskStatus.Running;
            else
                return _reloadSucceed ? TaskStatus.Success : TaskStatus.Failure;
        }
        public override void OnEnd()
        {
            base.OnEnd();
            weaponInput.reload = false;
            _reloadSucceed = false;
            _reloadExecuted = false;
            _reloadTimeline.EndAction -= ReloadEnded;
            launcher.ReloadCallback -= UpdateReloadSucceed;
        }
    }
}
