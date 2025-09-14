using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tests.Interaction
{
    internal abstract class TargetsCatcherBase : ITargetsCatcher
    {
        protected List<ITarget> targets;
        protected Action<IList<ITarget>> targetsChangedAction;
        public TargetsCatcherBase()
        {
            targets = NewTargetsContainer() ?? throw new NullReferenceException(nameof(targets));
        }
        protected virtual List<ITarget> NewTargetsContainer()
        {
            return new List<ITarget>();
        }
        public IReadOnlyList<ITarget> Targets => targets;

        public Action<IList<ITarget>> TargetsChangedAction { get => targetsChangedAction; set => targetsChangedAction = value; }
        protected virtual void AddTarget(ITarget target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            targets.Add(target);
            targetsChangedAction?.Invoke(targets);
        }
        protected virtual void RemoveTarget(ITarget target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (targets.Remove(target))
                targetsChangedAction?.Invoke(targets);
        }
        protected virtual void ClearAllTargets()
        {
            targets.TrimExcess();
            targets.Clear();
            targetsChangedAction?.Invoke(targets);
        }
        public abstract void Update();
    }
}
