using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Tests.Interaction
{
    public abstract class TargetsCatcherBase_New<T> : CatcherBase<T>, ITargetsCatcher_New<T> where T : ITarget_New
    {
        //    protected internal List<T> targets;
        //    protected internal Action<List<T>> targetsChangedAction;
        //    protected internal Action<T> targetCatchAction;
        //    protected internal Action<T> targetReleaseAction;
        //    protected internal bool enabled;
        //    public TargetsCatcherBase_New()
        //    {
        //        targets = NewTargetsContainer() ?? throw new NullReferenceException(nameof(targets));
        //    }
        //    protected virtual List<T> NewTargetsContainer()
        //    {
        //        return new List<T>();
        //    }
        //    public IReadOnlyList<T> Targets => targets;

        //    public Action<List<T>> TargetsChangedAction { get => targetsChangedAction; set => targetsChangedAction = value; }
        //    public virtual bool Enabled
        //    {
        //        get => enabled;
        //        set
        //        {
        //            enabled = value;
        //            if (!enabled)
        //                CleanAllTargets();
        //        }
        //    }

        //    public Action<T> TargetCaughtAction { get => targetCatchAction; set => targetCatchAction = value; }
        //    public Action<T> TargetReleaseAction { get => targetReleaseAction; set => targetReleaseAction = value; }

        //    internal protected virtual void AddTargetImpl(T target)
        //    {
        //        if (target == null)
        //            throw new ArgumentNullException(nameof(target));
        //        if (targets.Contains(target))
        //            return;
        //        targets.Add(target);
        //        targetCatchAction?.Invoke(target);
        //        targetsChangedAction?.Invoke(targets);
        //    }
        //    internal protected virtual void RemoveTargetImpl(T target)
        //    {
        //        if (target == null)
        //            throw new ArgumentNullException(nameof(target));
        //        if (targets.Remove(target))
        //        {
        //            targetReleaseAction?.Invoke(target);
        //            targetsChangedAction?.Invoke(targets);
        //        }
        //    }
        //    internal protected virtual void CleanAllTargets()
        //    {
        //        targets.TrimExcess();
        //        targets.Clear();
        //        targetsChangedAction?.Invoke(targets);
        //    }
        //    public virtual void Update()
        //    {

        //    }
        //    public virtual IEnumerator UpdateWithCoroutine()
        //    {
        //        yield return null;
        //    }
        //    public void TargetsToString(StringBuilder sb)
        //    {
        //        if (targets != null)
        //            for (int i = 0; i < targets.Count; i++)
        //            {
        //                var t = targets[i];
        //                sb.AppendLine($"{i} -> " + t.ToString());
        //            }
        //    }
        //    public override string ToString()
        //    {
        //        var sb = new StringBuilder();
        //        sb.AppendLine(base.ToString());
        //        sb.AppendLine("enabled: " + enabled);
        //        TargetsToString(sb);
        //        return sb.ToString();
        //    }

        //    public void AddTarget(T target)
        //    {
        //        AddTargetImpl(target);
        //    }

        //    public void RemoveTarget(T target)
        //    {
        //        RemoveTargetImpl(target);
        //    }
        //}
    }
}
