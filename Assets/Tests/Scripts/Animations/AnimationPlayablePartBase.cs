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
        internal bool initialized;
        //public virtual bool Enabled => enabled;

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
            if (setting is not OutputSetting os)
                throw new InvalidCastException();
            this.outputSetting.portNum = os.portNum;
            this.outputSetting.parent = os.parent;
            this.outputSetting.Weight = os.Weight;
        }
        public virtual void Dispose()
        {
            playablePart.Destroy();
        }
        [Obsolete]
        public abstract bool Initialize(PlayableGraph graph);
    }
}
