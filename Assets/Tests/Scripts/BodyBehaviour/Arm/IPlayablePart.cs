using System;
using Tests.Behaviours.Arm.Weapons;
using UnityEngine.Playables;

namespace Tests.Behaviours.Arm
{
    public interface IDynamicPlayablePart
    {
        public Playable GetPlayablePart(PlayableGraph graph);
        public IOutputSetting OutputSetting { get; set; }
        public Action<Playable> UpdateAction { get => null; set { } }
        public bool Enabled { get; set; }
    }
    public interface IPlayablePart
    {
        public Playable PlayablePart { get; }
    }
}
