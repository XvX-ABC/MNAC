using UnityEngine.Playables;

namespace Tests.Behaviours.Animations
{
    public abstract class AnimationPlayablePartBase : IAnimationPlayablePart
    {
        protected bool enabled;
        protected Playable playablePart;
        protected IOutputSetting outputSetting;
        protected AnimationPlayableNode node;
        public virtual bool Enabled => enabled;

        public Playable PlayablePart => playablePart;
        public virtual IOutputSetting OutputSetting
        {
            get => outputSetting;
            set => outputSetting = value;
        }
        public virtual IAnimationPlayablePartNode Node { get => node; }
        protected AnimationPlayablePartBase()
        {
            node = new(this);
        }

        public virtual void Dispose()
        {
            playablePart.Destroy();
        }
        public abstract bool Initialize(PlayableGraph graph);
    }
}
