using MNAC.Utilities.Blackboards;
using MNAC.Utilities.Timeline;
using MNAC.Utilities.Timeline.Events.Point;
using UnityEngine;

namespace MNAC.Weapons.Launcher
{
    internal abstract class LauncherEffector : LauncherEffectComponent
    {
        ITimeline _reloadTimeline;
        ITimeline _launchIntervalTimeline;
        IPointEvent _beforeLaunchEvent;
        [Range(0, 1)]
        [SerializeField]
        float _launchBeforeProportion;

        public override void Initialize(Blackboard blackboard)
        {
            base.Initialize(blackboard);
            owner.LaunchedCallback += WhenLaunch;
            owner.ReloadCallback += WhenReload;


            {
                _reloadTimeline = owner.ReloadTimeline;
                _reloadTimeline.StartAction += WhenReloadStart;
                _reloadTimeline.EndAction += WhenReloadEnd;
            }


            _launchIntervalTimeline = owner.LaunchingIntervalTimeline;
            _beforeLaunchEvent = (IPointEvent)_launchIntervalTimeline.AddPointEvent(_launchBeforeProportion, WhenLauncherBefore);
        }
        public override void Dispose()
        {
            base.Dispose();

            {
                _reloadTimeline.StartAction -= WhenReloadStart;
                _reloadTimeline.EndAction -= WhenReloadEnd;
            }

            owner.LaunchedCallback -= WhenLaunch;
            owner.ReloadCallback -= WhenReload;
            _launchIntervalTimeline.RemovePointEvent(_beforeLaunchEvent);
        }
        protected virtual void WhenLaunch(ILauncher launcher)
        {

        }
        protected virtual void WhenReloadStart(ILauncher launcher)
        {

        }
        protected virtual void WhenReloadEnd(ILauncher launcher)
        {

        }
        void WhenReloadStart(TimelineContext _)
        {
            WhenReloadStart(owner);
        }
        void WhenReloadEnd(TimelineContext _)
        {
            WhenReloadEnd(owner);
        }
        protected virtual void WhenReload(ILauncher launcher)
        {

        }
        void WhenLauncherBefore(TimelineContext ctx)
        {
            WhenLaunchBefore(owner);
        }
        protected virtual void WhenLaunchBefore(ILauncher launcher)
        {

        }
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
