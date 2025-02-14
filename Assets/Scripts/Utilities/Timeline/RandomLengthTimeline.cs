using UnityEngine;

namespace Assets.Scripts.Utilities.Timeline
{
    public class RandomLengthTimeline : Timeline
    {
        ITimeGeneration _timeGeneration;
        public RandomLengthTimeline(ITimeGeneration timeGeneration)
        {
            _timeGeneration = timeGeneration;
        }
        public RandomLengthTimeline(Vector2 range) : this(new RandomTimeGeneration(range))
        {

        }
        public override void Start()
        {
            if (_timeGeneration != null)
                duration = _timeGeneration.Time;
            base.Start();
        }
    }
}
