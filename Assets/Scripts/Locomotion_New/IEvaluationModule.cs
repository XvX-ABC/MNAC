namespace MNAC.Locomotion.Interfaces
{
    /// <summary>
    /// 评估模块接口，用于运动前的状态评估
    /// </summary>
    /// <typeparam name="TContext">运动上下文类型</typeparam>
    public interface IEvaluationModule<TContext> where TContext : ILocomotionContext
    {
        /// <summary>
        /// 模块是否启用
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// 更新评估逻辑
        /// </summary>
        void Update(TContext context);
    }
}