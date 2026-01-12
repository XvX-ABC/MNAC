using System;
using UnityEngine;

namespace Tests.Utilities.Timeline
{
    public class RandomLengthTimeline_V1 : Timeline
    {
        internal ITimeGenerator timeGenerator;
        public RandomLengthTimeline_V1(ITimeGenerator timeGeneration) : base(0)
        {
            timeGenerator = timeGeneration;
        }
        public RandomLengthTimeline_V1(Vector2 range) : this(new RandomTimeGeneration(range))
        {

        }
        public override void Restart()
        {
            if (timeGenerator != null)
                length = timeGenerator.Time;
            base.Restart();
        }
        public override void Reset()
        {
            base.Reset();
            time = 0;
        }
        public override bool UpdateLength(float newLength, bool runningCheck = true)
        {
            return true;
        }
    }
    public class RandomLengthTimeline : Timeline
    {
        internal ITimeGenerator timeGenerator;
        public RandomLengthTimeline(ITimeGenerator timeGeneration)
        {
            timeGenerator = timeGeneration;
        }
        public RandomLengthTimeline(Vector2 range) : this(new RandomTimeGeneration(range))
        {

        }
        public override void Restart()
        {
            if (timeGenerator != null)
                length = timeGenerator.Time;
            base.Restart();
        }
        public override void Reset()
        {
            base.Reset();
            time = 0;
        }
        public override bool UpdateLength(float newLength, bool runningCheck = true)
        {
            throw new NotImplementedException();
        }
    }
}
