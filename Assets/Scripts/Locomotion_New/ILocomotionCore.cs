using System.Collections.Generic;

namespace MNAC.Locomotion.Interfaces
{
    /// <summary>
    /// 运动核心接口，管理运动模块的生命周期
    /// </summary>
    /// <typeparam name="TContext">运动上下文类型</typeparam>
    /// <typeparam name="TModule">运动模块类型</typeparam>
    public interface ILocomotionCore<TContext, TModule>
        where TContext : ILocomotionContext
        where TModule : ILocomotionModule<TContext>
    {
        /// <summary>
        /// 运动上下文
        /// </summary>
        TContext Context { get; }

        /// <summary>
        /// 添加运动模块
        /// </summary>
        void AddModule(TModule module, bool enabled = false);

        /// <summary>
        /// 按优先级插入运动模块
        /// </summary>
        void AddModule_InsertByPriority(TModule module, bool enabled = false);

        /// <summary>
        /// 移除运动模块
        /// </summary>
        void RemoveModule(TModule module);

        /// <summary>
        /// 启用运动模块
        /// </summary>
        void EnableModule(TModule module);

        /// <summary>
        /// 禁用运动模块
        /// </summary>
        void DisableModule(TModule module);

        /// <summary>
        /// 检查是否包含指定模块
        /// </summary>
        bool Contains(TModule module);

        /// <summary>
        /// 更新所有运动模块
        /// </summary>
        void Update();
    }
}