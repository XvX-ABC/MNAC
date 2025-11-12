using System;
using System.Collections.Generic;

namespace Tests.Interaction

{
    public interface ITargetsCatcher_New<T> where T : ITarget
    {
        public bool Enabled { get; set; }
        public IReadOnlyList<T> Targets { get; }
        public Action<IList<T>> TargetsChangedAction { get; set; }
    }
}
