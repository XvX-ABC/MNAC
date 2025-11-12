using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Tests.Interaction
{
    public abstract class TargetsCatcherBase_MonoComponent : MonoBehaviour, ITargetsCatcher
    {
        protected class Catcher : TargetsCatcherBase
        {

        }
        protected Catcher catcher;
        protected TargetsCatcherBase_MonoComponent()
        {
            catcher = CreateCatcher();
        }
        protected virtual Catcher CreateCatcher()
        {
            return new Catcher();
        }
        public virtual bool Enabled
        {
            get => this.enabled;
            set
            {
                catcher.Enabled = value;
                this.enabled = value;
            }
        }

        public IReadOnlyList<ITarget> Targets => catcher.Targets;

        public Action<IList<ITarget>> TargetsChangedAction { get => catcher.TargetsChangedAction; set => catcher.TargetsChangedAction = value; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(base.ToString());
            sb.AppendLine("name: " + this.name);
            sb.AppendLine("enabled: " + this.enabled);
            catcher.TargetsToString(sb);
            return sb.ToString();
        }

    }
}
