using Assets.Scripts.Utilities.Timeline;
using Assets.Scripts.Utilities.Timeline.Event;
using Assets.Scripts.Utilities.Timeline.Event.Point;
using System;
using Tests.Weapons.Launcher;
using UnityEngine;

namespace Tests.BodyBehaviour.Arm
{
    [Serializable]
    internal class ArmReload
    {

        [SerializeField]
        float _clipLength;
        [SerializeField]
        string _speedMultiplierName;
        [SerializeField]
        string _enterParamName;
        [SerializeField]
        Animator _animator;

        ITimelineEvent _startEvent;
        ITimelineEvent _endEvent;
        ITimeline _timeline;
        ILauncher _launcher;
        //public ITimeline Timeline
        //{
        //    get => _timeline;
        //    set
        //    {
        //        if (value == null)
        //            throw new NullReferenceException(nameof(value));
        //        var length = value.Length;
        //        var multiplier = _clipLength / length;
        //        _animator.SetFloat(_speedMultiplierName, multiplier);

        //        if (_timeline != null)
        //        {
        //            _timeline.RemovePointEvent(_startEvent);
        //            _timeline.RemovePointEvent(_endEvent);
        //        }

        //        _timeline = value;
        //        _startEvent = _timeline.AddPointEvent(0, _ => _animator.SetBool(_enterParamName, true));
        //        _endEvent = _timeline.AddPointEvent(1, _ => _animator.SetBool(_enterParamName, false));
        //    }
        //}
        public ITimeline Timeline
        {
            get => _timeline;
        }
        public ILauncher Launcher
        {
            get => _launcher;
            set
            {
                var timeline = value.ReloadTimeline;
                var length = timeline.Length;
                var multiplier = _clipLength / length;
                _animator.SetFloat(_speedMultiplierName, multiplier);

                if (_timeline != null)
                {
                    _timeline.RemovePointEvent(_startEvent);
                    _timeline.RemovePointEvent(_endEvent);
                }

                _timeline = timeline;
                _startEvent = _timeline.AddPointEvent(0, _ => _animator.SetBool(_enterParamName, true));
                _endEvent = _timeline.AddPointEvent(1, _ => _animator.SetBool(_enterParamName, false));

                _launcher = value;

            }
        }
        public bool Continuing
        {
            get => _timeline.IsRunning;
        }
        public bool Start()
        {
            return _launcher.StartReload();
        }
        public bool End()
        {
            return _launcher.EndReload();
        }
    }
}