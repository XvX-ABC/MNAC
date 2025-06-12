using Assets.Scripts.Utilities.Timeline;
using Assets.Scripts.Utilities.Timeline.Event;
using Assets.Scripts.Utilities.Timeline.Event.Point;
using Assets.Scripts.Utilities.Timeline.Event.Range;
using System;
using Tests.Input;
using Tests.Weapons;
using Tests.Weapons.Launcher;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.Rendering;

namespace Tests.BodyBehaviour.Arm
{
    [Serializable]
    internal class AimToReloadTransition
    {
        [SerializeField]
        AnimationCurve _curve;
        [SerializeField]
        float _durationTime;


        internal ArmAim aim;
        internal ArmReloadAnimation reloadAnimation;

        ITimeline _timeline_ator;
        ITimeline _timeline_rtoa;
        ITimeline _reloadTimeline;
        ITimelineEvent _reloadEndEvent;
        ITimelineEvent _continuingEvent;
        ITimelineEvent _pauseEvent;
        ILauncher _launcher;
        internal void Initialize(ArmAim aim, ArmReloadAnimation reloadAnimation)
        {
            this.aim = aim ?? throw new ArgumentNullException(nameof(aim));
            this.reloadAnimation = reloadAnimation ?? throw new ArgumentNullException(nameof(reloadAnimation));
            InitializeTimelines(_durationTime);
            aim.Weight = 1;
        }

        public ILauncher Target
        {
            get => _launcher;
            set
            {
                if (value == null)
                    throw new NullReferenceException(nameof(value));
                var timeline = value.ReloadTimeline;
                if (_reloadEndEvent != null)
                {
                    _reloadTimeline.RemoveEvent(_reloadEndEvent);
                    _reloadTimeline.RemoveEvent(_continuingEvent);
                    _reloadTimeline.RemoveEvent(_pauseEvent);
                }


                _reloadEndEvent = timeline.AddPointEvent(1, _ => _timeline_rtoa.Start());
                _continuingEvent = timeline.AddPointEvent(0, _ => reloadAnimation.Continue());
                _pauseEvent = timeline.AddPointEvent(1, _ => reloadAnimation.Pause());


                _reloadTimeline = timeline;

                reloadAnimation.DurationTime = timeline.Length;
                _launcher = value;
            }
        }
        public bool Continuing => _timeline_ator.IsRunning || _timeline_rtoa.IsRunning || _reloadTimeline.IsRunning;
        public bool Begin()
        {
            if (!aim.Continuing || _timeline_ator.IsRunning || _timeline_rtoa.IsRunning || _reloadTimeline.IsRunning)
                return false;
            _timeline_ator.Start();
            return true;
        }

        public bool End()
        {
            if (aim.Continuing && !_timeline_ator.IsRunning)
                return false;


            if (_timeline_ator.IsRunning)
                _timeline_ator.EarlyEnd();
            else if (_reloadTimeline.IsRunning)
                _launcher.EndReload();
            else if (_timeline_rtoa.IsRunning)
                _timeline_rtoa.EarlyEnd();

            aim.Weight = 1;
            if (!aim.Continuing)
                aim.Begin();
            return true;
        }

        void InitializeTimelines(float durationTime)
        {
            _timeline_ator = new Timeline(durationTime);
            _timeline_ator.AddRangeEvent(0, 1, ctx =>
            {
                var weight = _curve.Evaluate(ctx.Proportion);
                aim.Weight = weight;
            });
            _timeline_ator.AddPointEvent(0, _ =>
            {
                reloadAnimation.Pause();
            });
            _timeline_ator.AddPointEvent(1, _ =>
            {
                if (!aim.End())
                    throw new Exception("Try to end aim behaviour failed.");
                if (!_launcher.StartReload())
                    throw new Exception("Try to start reload behaviour failed.");
            });


            _timeline_rtoa = new Timeline(durationTime);
            _timeline_rtoa.AddRangeEvent(0, 1, ctx =>
            {
                var weight = _curve.Evaluate(1 - ctx.Proportion);
                aim.Weight = weight;
            });
            _timeline_rtoa.AddPointEvent(0, _ =>
            {
                if (!aim.Begin())
                    throw new Exception("Try to start aim behaviour failed.");
            });
        }
        public void OnUpdate()
        {

            if (_timeline_ator.IsRunning)
                _timeline_ator.OnUpdate(Time.deltaTime);
            if (_timeline_rtoa.IsRunning)
                _timeline_rtoa.OnUpdate(Time.deltaTime);
        }
    }
}