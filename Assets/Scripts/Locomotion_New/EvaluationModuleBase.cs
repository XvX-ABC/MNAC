namespace MNAC.Locomotion.Base
{
    using Interfaces;

    /// <summary>
    /// 评估模块抽象基类
    /// </summary>
    /// <typeparam name="TContext">运动上下文类型</typeparam>
    public abstract class EvaluationModuleBase<TContext> : IEvaluationModule<TContext>
        where TContext : ILocomotionContext
    {
        protected bool enabled;

        public virtual bool Enabled
        {
            get => enabled;
            set => enabled = value;
        }

        public virtual void Update(TContext context) { }
    }
}