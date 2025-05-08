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
        internal ArmReload reload;


        ITimeline _timeline_ator;
        ITimeline _timeline_rtoa;
        ITimelineEvent _reloadEndEvent;
        //ILauncher _launcher;
        internal void Initialize(ArmAim aim, ArmReload reload)
        {
            this.aim = aim ?? throw new ArgumentNullException(nameof(aim));
            this.reload = reload ?? throw new ArgumentNullException(nameof(reload));
            InitializeTimelines(_durationTime);
            aim.Weight = 1;
        }

        //internal ITimeline reloadTimeline
        //{
        //    set
        //    {
        //        var timeline = reload.Timeline;
        //        if (timeline != null)
        //            timeline.RemovePointEvent(_reloadEndEvent);

        //        reload.Timeline = timeline;
        //        timeline = value;
        //        _reloadEndEvent = timeline.AddPointEvent(1, _ => _timeline_rtoa.Start());
        //    }

        //}
        public ILauncher Target
        {
            get => reload.Launcher;
            set
            {
                if (value == null)
                    throw new NullReferenceException(nameof(value));
                var timeline = value.ReloadTimeline;
                if (_reloadEndEvent != null)
                    reload.Timeline.RemoveEvent(_reloadEndEvent);
                _reloadEndEvent = timeline.AddPointEvent(1, _ => _timeline_rtoa.Start());

                reload.Launcher = value;
            }
        }
        public bool Continuing => _timeline_ator.IsRunning || _timeline_rtoa.IsRunning;
        public bool Begin()
        {
            if (_timeline_ator.IsRunning || _timeline_rtoa.IsRunning || reload.Continuing)
                return false;
            _timeline_ator.Start();
            return true;
        }

        public bool End()
        {
            if (aim.Continuing && !_timeline_ator.IsRunning)
                return false;


            if (_timeline_ator.IsRunning)
                _timeline_ator.Stop();
            else if (reload.Continuing)
                //_launcher.EndReload();
                reload.End();
            else if (_timeline_rtoa.IsRunning)
                _timeline_rtoa.Stop();

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
            _timeline_ator.AddPointEvent(1, _ =>
            {
                if (!aim.End())
                    throw new Exception("Try to end aim behaviour failed.");
                if (!reload.Start())
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