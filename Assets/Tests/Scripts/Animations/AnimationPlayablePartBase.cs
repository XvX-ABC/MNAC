using System;
using UnityEngine.Playables;

namespace Tests.Animations
{
    public abstract class AnimationPlayablePartBase : IAnimationPlayablePart
    {
        protected bool enabled;
        protected PlayableGraph graph;
        protected Playable playablePart;
        internal OutputSetting outputSetting;
        protected AnimationPlayableNode node;

        public Playable PlayablePart => playablePart;
        public virtual IOutputSetting OutputSetting
        {
            get => outputSetting;
            set
            {
                UpdateOutputSetting(value);
            }
        }
        public virtual IAnimationPlayablePartNode Node { get => node; }

        protected AnimationPlayablePartBase(PlayableGraph graph)
        {
            node = new(this);
            outputSetting = new OutputSetting();
            this.graph = graph;
        }
        protected void UpdateOutputSetting(IOutputSetting setting)
        {
            this.outputSetting.PortNum = setting.PortNum;
            this.outputSetting.Parent = setting.Parent;
            this.outputSetting.Weight = setting.Weight;

        }

        public virtual void Dispose()
        {
            if (!playablePart.IsNull())
                playablePart.Destroy();
        }
    }
}
