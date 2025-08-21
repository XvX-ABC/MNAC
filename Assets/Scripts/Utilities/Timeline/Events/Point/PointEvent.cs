using System;
using Utilities.Timeline;

namespace Utilities.Timeline.Events.Point
{
    public abstract class PointEvent : IPointEvent
    {
        protected Func<TimelineContext, bool> triggeredFunc;
        protected PointEvent(float triggeredProportion)
        {
            triggeredFunc = context =>
            {
                return context.NormalizedTime >= TriggeredProportion;
            };
            _triggeredProportion = triggeredProportion;
        }
        protected float _triggeredProportion;


        public float TriggeredProportion { get => _triggeredProportion; }
        public virtual Func<TimelineContext, bool> Trigger { get => triggeredFunc; }

        public abstract void Execute(TimelineContext context);
        public virtual void Reset() { }
    }
}
