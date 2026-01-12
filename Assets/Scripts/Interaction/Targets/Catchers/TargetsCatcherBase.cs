using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Tests.Interaction
{
    public abstract class TargetsCatcherBase : ITargetsCatcher
    {
        protected internal List<ITarget_Obsolete> targets;
        protected internal Action<IList<ITarget_Obsolete>> targetsChangedAction;
        protected internal bool enabled;
        public TargetsCatcherBase()
        {
            targets = NewTargetsContainer() ?? throw new NullReferenceException(nameof(targets));
        }
        protected virtual List<ITarget_Obsolete> NewTargetsContainer()
        {
            return new List<ITarget_Obsolete>();
        }
        public IReadOnlyList<ITarget_Obsolete> Targets => targets;

        public Action<IList<ITarget_Obsolete>> TargetsChangedAction { get => targetsChangedAction; set => targetsChangedAction = value; }
        public virtual bool Enabled
        {
            get => enabled;
            set
            {
                enabled = value;
                if (!enabled)
                    CleanAllTargets();
            }
        }

        internal protected virtual void AddTarget(ITarget_Obsolete target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            targets.Add(target);
            targetsChangedAction?.Invoke(targets);
        }
        internal protected virtual void RemoveTarget(ITarget_Obsolete target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (targets.Remove(target))
                targetsChangedAction?.Invoke(targets);
        }
        internal protected virtual void CleanAllTargets()
        {
            targets.TrimExcess();
            targets.Clear();
            targetsChangedAction?.Invoke(targets);
        }
        public virtual void Update()
        {

        }
        public virtual IEnumerator UpdateWithCoroutine()
        {
            yield return null;
        }
        public void TargetsToString(StringBuilder sb)
        {
            if (targets != null)
                for (int i = 0; i < targets.Count; i++)
                {
                    var t = targets[i];
                    sb.AppendLine($"{i} -> " + t.ToString());
                }
        }
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(base.ToString());
            sb.AppendLine("enabled: " + enabled);
            TargetsToString(sb);
            return sb.ToString();
        }
    }
}
