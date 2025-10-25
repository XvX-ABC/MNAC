using System;
using System.Collections.Generic;

namespace Tests.Interaction
{
    public interface ITargetsCatcher
    {
        public bool Enabled { get; set; }
        public IReadOnlyList<ITarget> Targets { get; }
        public Action<IList<ITarget>> TargetsChangedAction { get; set; }
    }
}
