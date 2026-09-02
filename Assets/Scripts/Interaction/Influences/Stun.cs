using System;
using MNAC.Interaction.Influences;
using MNAC.Utilities.Timeline;
using UnityEngine;

namespace MNAC.Interaction.Influence
{
    public class Stun : InfluenceBase
    {
        internal ITimeline timeline;
        public Stun()
        {
            timeline = new Timeline(0);
            timeline.EndAction += _ => base.Enabled = false;
        }
        public float DurationTime
        {
            get => timeline.Length;
            set => timeline.UpdateLength(Mathf.Max(0, value));
        }
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

        public ITimeline Timeline { get => timeline; }

        public override void Update()
        {
            timeline.OnUpdate(Time.deltaTime);
        }

    }
}
