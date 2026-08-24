namespace MNAC.Locomotion.Base
{
    using Interfaces;

    /// <summary>
    /// 运动模块抽象基类，提供默认实现
    /// </summary>
    /// <typeparam name="TContext">运动上下文类型</typeparam>
    public abstract class LocomotionModuleBase<TContext> : ILocomotionModule<TContext>
        where TContext : ILocomotionContext
    {
        protected bool enabled;
        protected int priority;

        public LocomotionModuleBase(int priority = 0)
        {
            this.priority = priority;
        }

        public virtual bool Enabled
        {
            get => enabled;
            set => enabled = value;
        }

        public int Priority => priority;

        public virtual TContext OnStart(TContext context) { return context; }
        public virtual TContext OnUpdate(TContext context) { return context; }
        public virtual TContext OnEnd(TContext context) { return context; }

        // 封装原有的生命周期管理
        public TContext Start(TContext context)
        {
            context = OnStart(context);
            return context;
        }

        public TContext Update(TContext context)
        {
            context = OnUpdate(context);
            return context;
        }

        public TContext End(TContext context)
        {
            context = OnEnd(context);
            return context;
        }
    }
}