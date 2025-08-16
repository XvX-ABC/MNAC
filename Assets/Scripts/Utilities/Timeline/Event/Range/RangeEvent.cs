using System;

namespace Assets.Scripts.Utilities.Timeline.Event.Range
{
    public abstract class RangeEvent : IRangeEvent
    {
        protected Func<TimelineContext, bool> triggeredFunc;

        protected RangeEvent(float durationProportion, float triggeredProportion)
        {
            triggeredFunc = context =>
            {
                var proportion = context.NormalizedTime;
                var startProportion = TriggeredProportion;
                var endProportion = startProportion + DurationProportion;
                //return proportion >= startProportion && proportion < endProportion;
                return proportion > startProportion && proportion <= endProportion;
            };
            this.durationProportion = durationProportion;
            this.triggeredProportion = triggeredProportion;
        }

        protected float durationProportion;
        protected float triggeredProportion;
        public Func<TimelineContext, bool> Trigger => triggeredFunc;

        public float DurationProportion { get => durationProportion; }
        public float TriggeredProportion { get => triggeredProportion; }

        public abstract void Execute(TimelineContext context);
        public virtual void Reset() { }
    }
}
