namespace MNAC.TPhysics.Locomotion
{
    /// 运动模块基类：生命周期追踪由 LocomotionCore 内部的私有状态机负责，
    /// 模块自身不保存状态，只暴露 Start/Update/End → On* 钩子的转发。
    public abstract class LocomotionModuleBase : ILocomotionModule
    {
        protected bool enabled;
        protected int priority;

        protected LocomotionModuleBase(int priority = 0)
        {
            this.priority = priority;
        }

        public virtual bool Enabled
        {
            get => enabled;
            set => enabled = value;
        }

        public int Priority => priority;

        // 生命周期钩子默认透传，模块只覆盖需要处理的那一个。
        public virtual Context OnStart(Context context) => context;
        public virtual Context OnUpdate(Context context) => context;
        public virtual Context OnEnd(Context context) => context;

        public Context Start(Context context) => OnStart(context);
        public Context Update(Context context) => OnUpdate(context);
        public Context End(Context context) => OnEnd(context);
    }
}
