using System;
using Tests.Behaviours.Animations;
using UnityEngine.Playables;

namespace Tests.Behaviours.Arms
{
    public interface IDynamicPlayablePart
    {
        public Playable GetPlayablePart(PlayableGraph graph);
        public IOutputSetting OutputSetting { get; set; }
        [Obsolete]
        public Action<Playable> UpdateAction { get => null; set { } }
        public bool Enabled { get; set; }
    }
}
