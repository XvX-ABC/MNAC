using System.Collections.Generic;
using Tests.Utilities.Timeline.Events;

namespace Tests.Utilities.Timeline.Events.Point
{
    public interface IPointEvent : ITimelineEvent
    {
        public class Comparer : IComparer<IPointEvent>
        {
            public int Compare(IPointEvent x, IPointEvent y)
            {
                var v0 = x.TriggeredProportion;
                var v1 = y.TriggeredProportion;
                if (v0 < v1)
                    return -1;
                else if (v0 > v1)
                    return 1;
                return 0;
            }
        }
        public float TriggeredProportion { get; set; }
    }
}
