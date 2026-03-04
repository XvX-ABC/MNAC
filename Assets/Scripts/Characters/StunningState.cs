using System;
using MNAC.Utilities.Timeline;

namespace MNAC.Characters
{
    internal class StunningState : CharacterBehaviourStateBase
    {
        public StunningState(ITimeline timeline, bool enabled = true) : base("stunning", 0, enabled)
        {
            base.timeline = timeline ?? throw new ArgumentNullException(nameof(timeline));
        }
    }
}
