using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Weapons.Launcher;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Utilities.Timeline.Events;

namespace Utilities.Timeline
{
    public class TimelinesGroup : ITimeline
    {
        ITimeline[] _timelines;
        ITimeline _lastEndTimeline;
        ILauncher[] _launchers;
        Func<ILauncher, ITimeline> _timelineGetFunc;
        internal ITimeline lastEndTimeline
        {
            get
            {
                if (_lastEndTimeline == null)
                    _lastEndTimeline = timelines.OrderByDescending(t => t.Length).First();
                return _lastEndTimeline;
            }
        }
        internal ITimeline[] timelines
        {
            get
            {
                if (_timelines == null)
                {
                    _timelines = new ITimeline[_launchers.Length];
                    for (int i = 0; i < _launchers.Length; i++)
                    {
                        var l = _launchers[i];
                        var t = _timelineGetFunc(l) ?? throw new NullReferenceException($"{nameof(_launchers)}[{i}]");
                        _timelines[i] = t;
                    }
                }
                return _timelines;
            }
        }
        public TimelinesGroup(params ITimeline[] timelines)
        {
            for (int i = 0; i < timelines.Length; i++)
            {
                var t = timelines[i];
                if (t == null)
                    throw new ArgumentNullException($"timelines[{i}]");
            }
            _timelines = timelines;
        }
        public TimelinesGroup(Func<ILauncher, ITimeline> getFunc, params ILauncher[] launchers)
        {
            if (launchers.Any(l => l == null))
                throw new ArgumentNullException($"There have a null element in the argument '{nameof(launchers)}'");
            _launchers = launchers;
            _timelineGetFunc = getFunc ?? throw new NullReferenceException(nameof(getFunc));
        }

        public bool IsRunning => lastEndTimeline.IsRunning;

        public float Time => lastEndTimeline.Time;
        public float NormalizedTime
        {
            get => lastEndTimeline.Length == 0 ? 1 : lastEndTimeline.Time / lastEndTimeline.Length;
        }
        public float Length
        {
            get
            {
                if (timelines.Length == 0)
                    return 0;
                return lastEndTimeline.Length;
            }
        }
        public Action<TimelineContext> StartAction
        {
            get
            {
                if (timelines.Length == 0)
                    return null;
                return lastEndTimeline.StartAction;
            }
            set
            {
                lastEndTimeline.StartAction = value;
            }
        }
        public Action<float> UpdateAction
        {
            get
            {
                if (timelines.Length == 0)
                    return null;
                return lastEndTimeline.UpdateAction;
            }
            set
            {
                lastEndTimeline.UpdateAction = value;
            }
        }
        public Action<TimelineContext> EndAction
        {
            get
            {
                if (timelines.Length == 0)
                    return null;
                return lastEndTimeline.EndAction;
            }
            set
            {
                lastEndTimeline.EndAction = value;
            }
        }
        public bool AddEvent(ITimelineEvent evt)
        {
            var t = lastEndTimeline;
            if (!t.AddEvent(evt))
            {
                RemoveEvent(evt);
                return false;
            }
            return true;
        }

        public void Start()
        {
            foreach (var l in timelines)
                l.Start();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var l in timelines)
                l.OnUpdate(deltaTime);
        }

        public bool RemoveEvent(ITimelineEvent evt)
        {
            var t = lastEndTimeline;
            if (!t.RemoveEvent(evt))
                return false;
            return true;
        }

        public void Restart()
        {
            _lastEndTimeline = timelines.OrderByDescending(t => t.Length).First();
            foreach (var l in timelines)
                l.Restart();
        }

        public void Pause()
        {
            foreach (var l in timelines)
                l.Pause();
        }
        public bool UpdateLength(float newLength)
        {
            throw new NotImplementedException();
        }

        public void EarlyEnd()
        {
            throw new NotImplementedException();
        }

        public void End()
        {
            throw new NotImplementedException();
        }

        public bool SetNormalizedTime(float normalizedTime)
        {
            throw new NotImplementedException();
        }

        public bool SetTime(float time)
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
