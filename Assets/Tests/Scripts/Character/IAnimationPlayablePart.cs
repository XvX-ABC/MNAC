using System;
using Tests.Behaviours.Arm.Weapons;
using UnityEngine.Playables;

namespace Tests.Character
{
    public interface IAnimationPlayablePart : IDisposable
    {
        public bool Enabled { get; }
        public bool Initialize(PlayableGraph graph);
        public Playable PlayablePart { get; }
        public IOutputSetting OutputSetting { get; set; }
        public IAnimationPlayablePartNode Node { get; }
    }
}
