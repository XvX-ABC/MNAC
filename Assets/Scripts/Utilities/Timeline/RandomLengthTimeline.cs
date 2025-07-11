using System;
using UnityEngine;

namespace Assets.Scripts.Utilities.Timeline
{
    public class RandomLengthTimeline : Timeline
    {
        internal ITimeGenerator timeGenerator;
        public RandomLengthTimeline(ITimeGenerator timeGeneration)
        {
            this.timeGenerator = timeGeneration;
        }
        public RandomLengthTimeline(Vector2 range) : this(new RandomTimeGeneration(range))
        {

        }
        public override void Start()
        {
            if (timeGenerator != null)
                length = timeGenerator.Time;
            base.Start();
        }
        protected override void Reset()
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
