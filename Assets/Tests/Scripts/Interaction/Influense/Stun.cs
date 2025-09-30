using System;
using UnityEngine;
using Utilities.Timeline;

namespace Tests.Interaction.Influence
{
    public class Health : InfluenceBase
    {
        float _minPoint;
        float _point;
        private float _maxPoint;

        public float MaxPoint { get => _maxPoint; set => _maxPoint = value; }
        public float MinPoint { get => _minPoint; set => _minPoint = value; }
        public float Point { get => _point; set => _point = Mathf.Max(_minPoint, value); }
        public override string Name => "health";
        public bool IsAlive { get => _point <= _minPoint; }
    }
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
