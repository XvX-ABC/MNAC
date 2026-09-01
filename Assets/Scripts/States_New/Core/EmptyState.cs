using System;

namespace MNAC.StatesNew
{
    /// <summary>非泛型薄壳，等价于 <see cref="EmptyState{T}"/> with T=object。</summary>
    public class EmptyState : EmptyState<object>
    {
        public EmptyState(string name = "Empty") : base(name)
        {
        }
    }

    /// <summary>空状态：生命周期全 no-op，可作占位/初始态。</summary>
    public class EmptyState<T> : StateBase<T>
    {
        public EmptyState(string name = "Empty") : base(name)
        {
        }

        public override void OnEnter()
        {
        }

        public override void OnUpdate(float deltaTime)
        {
        }

        public override void OnExit()
        {
        }
    }
}
