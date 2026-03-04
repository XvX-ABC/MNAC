using System;
using UnityEngine.Playables;

namespace MNAC.Animations
{
    public interface IDynamicPlayablePart
    {
        public Playable GetPlayablePart(PlayableGraph graph);
        public IOutputSetting OutputSetting { get; set; }
        [Obsolete]
        public Action<Playable> UpdateAction { get => null; set { } }
        [Obsolete]
        public bool Enabled { get; set; }
    }
}
