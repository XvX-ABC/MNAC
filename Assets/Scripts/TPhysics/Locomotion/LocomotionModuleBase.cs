using System;
using System.Reflection;
using System.Security.Policy;
using Unity.Properties;
using UnityEngine.Rendering;

namespace MNAC.TPhysics.Locomotion
{
    public abstract class LocomotionModuleBase : ILocomotionModule
    {

        protected bool enabled;
        protected LocomotionModuleState state;
        protected int priority;
        public LocomotionModuleBase(int priority = 0)
        {
            state = LocomotionModuleState.Ready;
            this.priority = priority;
        }
        public virtual bool Enabled
        {
            get => enabled;
            set => enabled = value;
        }

        public LocomotionModuleState State { get => state; }
        public int Priority { get => priority; }

        public virtual Context OnStart(Context context) { return context; }
        public virtual Context OnUpdate(Context context) { return context; }
        public virtual Context OnEnd(Context context) { return context; }

        public Context Start(Context context)
        {
            context = OnStart(context);
            state = LocomotionModuleState.Started;
            return context;
        }

        public Context Update(Context context)
        {
            context = OnUpdate(context);
            state = LocomotionModuleState.Updating;
            return context;
        }

        public Context End(Context context)
        {
            context = OnEnd(context);
            state = LocomotionModuleState.Ended;
            return context;
        }
    }
}