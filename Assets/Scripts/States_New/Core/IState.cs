using System;

namespace MNAC.StatesNew
{
    /// <summary>
    /// 状态接口。状态只负责自身生命周期，不持有转移表——
    /// 转移由 <see cref="StateMachine{T}"/> 统一持有（转移归属机器，状态零存储）。
    /// </summary>
    public interface IState<T>
    {
        /// <summary>状态名（日志/调试用）。</summary>
        string Name { get; }

        /// <summary>实例唯一标识。</summary>
        Guid Id { get; }

        /// <summary>是否启用。转移检测时目标状态必须 Enabled 才生效。</summary>
        bool Enabled { get; set; }

        /// <summary>共享上下文。机器 AddState 时注入。</summary>
        T Context { get; set; }

        void OnEnter();
        void OnUpdate(float deltaTime);
        void OnExit();
    }

    /// <summary>非泛型薄壳，等价于 <see cref="IState{T}"/> with T=object。</summary>
    public interface IState : IState<object>
    {
    }
}
