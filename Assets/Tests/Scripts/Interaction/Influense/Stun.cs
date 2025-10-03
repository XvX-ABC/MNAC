using System;
using UnityEngine;
using Utilities.Timeline;

namespace Tests.Interaction.Influence
{
    public class Stun : InfluenceBase
    {
        internal ITimeline timeline;
        public Stun()
        {
            timeline = new Timeline_V1(0);
            timeline.EndAction += _ => base.Enabled = false;
        }
        public float DurationTime
        {
            get => timeline.Length;
            set => timeline.UpdateLength(Mathf.Max(0, value));
        }
        //public float StunningDurationTime
        //{
        //    get => timeline.Length;
        //    set => timeline.UpdateLength(Mathf.Max(0, value));
        //}
        public override bool Enabled
        {
            get => base.Enabled;
            set
            {
                if (value)
                    timeline.Restart();
                else if (timeline.IsRunning)
                    timeline.End();
                base.Enabled = value;
            }
        }
        public override string Name => "stun";
        public override void Update()
        {
            timeline.OnUpdate(Time.deltaTime);
        }

    }
}
