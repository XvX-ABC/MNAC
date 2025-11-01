using System;
using UnityEngine;

namespace Tests.Utilities.Timeline
{
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
        public override bool UpdateLength(float newLength)
        {
            throw new NotImplementedException();
        }
    }
}
