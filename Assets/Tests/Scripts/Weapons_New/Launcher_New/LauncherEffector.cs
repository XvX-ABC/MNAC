using Tests.Utilities.Blackboards;
using Tests.Utilities.Timeline;
using Tests.Utilities.Timeline.Events.Point;
using UnityEngine;

namespace Tests.Weapons_New.Launcher
{
    internal abstract class LauncherEffector : LauncherEffectComponent
    {
        protected ILauncher launcher;
        ITimeline _launchIntervalTimeline;
        IPointEvent _beforeLaunchEvent;
        [Range(0, 1)]
        [SerializeField]
        float _launchBeforeProportion;

        protected virtual void OnEnable()
        {
            //launcher.LaunchAction += WhenLaunch;
            //launcher.ReloadAction += WhenReload;
            //_beforeLaunchEvent = (IPointEvent)_launchIntervalTimeline.AddPointEvent(_launchBeforeProportion, WhenBeforeLaunch);
        }
        protected virtual void OnDisable()
        {
            //launcher.LaunchAction -= WhenLaunch;
            //launcher.ReloadAction -= WhenReload;
            //_launchIntervalTimeline.RemovePointEvent(_beforeLaunchEvent);
        }
        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            blackboard.TryReadValueOrThrowException<ILauncher>(LauncherEffectComponent.OwnerLauncher, out launcher);
            launcher.LaunchedCallback += WhenLaunch;
            launcher.ReloadCallback += WhenReload;
            _launchIntervalTimeline = launcher.LaunchingIntervalTimeline;
            _beforeLaunchEvent = (IPointEvent)_launchIntervalTimeline.AddPointEvent(_launchBeforeProportion, WhenBeforeLaunch);
        }
        public override void Dispose()
        {
            base.Dispose();
            launcher.LaunchedCallback -= WhenLaunch;
            launcher.ReloadCallback -= WhenReload;
            _launchIntervalTimeline.RemovePointEvent(_beforeLaunchEvent);
        }
        protected abstract void WhenLaunch(ILauncher launcher);
        protected abstract void WhenReload(ILauncher launcher);
        void WhenBeforeLaunch(TimelineContext ctx)
        {
            WhenBeforeLaunch(launcher);
        }
        protected abstract void WhenBeforeLaunch(ILauncher launcher);
        public virtual float LaunchBeforeProportion
        {
            get => _launchBeforeProportion;
            set
            {
                _launchBeforeProportion = value;
                if (_beforeLaunchEvent != null)
                    _beforeLaunchEvent.TriggeredProportion = _launchBeforeProportion;
            }
        }
    }
}
