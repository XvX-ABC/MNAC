using System;
using UnityEngine.Playables;

namespace Tests.Animations
{
    public interface IAnimationPlayablePart : IDisposable
    {
        //public bool Initialized { get; }
        //public bool Initialize(PlayableGraph graph);
        public Playable PlayablePart { get; }
        public IOutputSetting OutputSetting { get; set; }
        public IAnimationPlayablePartNode Node { get; }
    }
}
