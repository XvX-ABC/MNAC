using System;

namespace Assets.Scripts.Utilities.Timeline.Event.Point
{
    public abstract class PointEvent : IPointEvent
    {
        protected Func<TimelineContext, bool> triggeredFunc;
        protected PointEvent(float triggeredProportion)
        {
            triggeredFunc = context =>
            {
                return context.Proportion >= TriggeredProportion;
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
