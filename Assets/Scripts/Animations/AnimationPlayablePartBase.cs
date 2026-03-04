using System;
using UnityEngine.Playables;

namespace MNAC.Animations
{
    public abstract class AnimationPlayablePartBase : IAnimationPlayablePart
    {
        protected PlayableGraph graph;
        protected Playable playablePart;
        OutputSetting _outputSetting;
        protected AnimationPlayableNode node;
        protected internal IOutputSetting outputSetting
        {
            get => _outputSetting;
        //HACK: 临时处理，后续需要修改
            set => _outputSetting = value as OutputSetting;
        }
        public Playable PlayablePart => playablePart;
        public virtual IOutputSetting OutputSetting
        {
            get => _outputSetting;
            set
            {
                UpdateOutputSetting(value);
            }
        }
        public virtual IAnimationPlayablePartNode Node { get => node; }

        protected AnimationPlayablePartBase(PlayableGraph graph)
        {
            node = CreateNode();
            _outputSetting = new OutputSetting();
            this.graph = graph;
        }
        protected virtual AnimationPlayableNode CreateNode()
        {
            return new(this);
        }
        protected void UpdateOutputSetting(IOutputSetting setting)
        {
            this._outputSetting.PortNum = setting.PortNum;
            this._outputSetting.Parent = setting.Parent;
            this._outputSetting.Weight = setting.Weight;

        }

        public virtual void Dispose()
        {
        }

        ~AnimationPlayablePartBase()
        {
            if (!playablePart.IsNull())
                playablePart.Destroy();
        }
    }
}
