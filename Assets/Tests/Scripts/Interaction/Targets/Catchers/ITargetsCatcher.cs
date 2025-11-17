using System;
using System.Collections.Generic;

namespace Tests.Interaction

{
    public interface ITargetsCatcher
    {
        public bool Enabled { get; set; }
        public IReadOnlyList<ITarget_Obsolete> Targets { get; }
        public Action<IList<ITarget_Obsolete>> TargetsChangedAction { get; set; }
    }
}
