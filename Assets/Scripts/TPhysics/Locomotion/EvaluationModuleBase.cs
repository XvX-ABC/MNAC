namespace MNAC.TPhysics.Locomotion
{
    /// 评估模块基类：启用开关 + 每帧 Update 钩子。
    public abstract class EvaluationModuleBase : IEvaluationModule
    {
        protected bool enabled;

        public virtual bool Enabled
        {
            get => enabled;
            set => enabled = value;
        }

        public virtual Context Update(Context context) => context;
    }
}
