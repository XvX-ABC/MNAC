using System;
using UnityEngine;
using Utilities.Timeline;

namespace Tests.Interaction.Influence
{
    public class StunReceptor : InfluenceReceptorBase, IInfluenceReceptor
    {
        internal ITimeline timeline;
        public StunReceptor()
        {
            timeline = new Timeline_V1(0);
            timeline.EndAction += _ => base.Enabled = false;
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
        public override string Name => "stunning_receptor";
        public override bool TryGetValue<T>(out T value, object key = null)
        {
            value = default;
            if (key == null)
            {
                var r = typeof(T) == typeof(float);
                value = r ? (T)Convert.ChangeType(timeline.Length, typeof(T)) : default;
                return r;
            }
            return false;
        }

        public override bool TrySetValue<T>(T value, object key = null)
        {
            if (key == null)
            {
                if (value is float f)
                {
                    timeline.UpdateLength(f);
                    return true;
                }
            }
            return false;
        }

        public override void Update()
        {
            timeline.OnUpdate(Time.deltaTime);
        }

    }
}
