using System;

namespace MNAC.Locomotion.Interfaces
{
    /// <summary>
    /// 运动模块接口，使用泛型约束确保类型安全
    /// </summary>
    /// <typeparam name="TContext">运动上下文类型</typeparam>
    public interface ILocomotionModule<TContext> where TContext : ILocomotionContext
    {
        /// <summary>
        /// 模块是否启用
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// 模块优先级（数值越大优先级越高）
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// 模块启动时调用
        /// </summary>
        TContext Start(TContext context);

        /// <summary>
        /// 模块每帧更新时调用
        /// </summary>
        TContext Update(TContext context);

        /// <summary>
        /// 模块结束时调用
        /// </summary>
        TContext End(TContext context);
    }
}