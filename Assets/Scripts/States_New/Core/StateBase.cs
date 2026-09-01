using System;

namespace MNAC.StatesNew
{
    /// <summary>非泛型薄壳，等价于 <see cref="StateBase{T}"/> with T=object。</summary>
    public abstract class StateBase : StateBase<object>
    {
        public StateBase(string name, bool enabled = true) : base(name, enabled)
        {
        }
    }

    /// <summary>
    /// 状态基类。只实现元数据（Name/Id/Enabled/Context），生命周期由子类实现。
    /// 注意：转移表归状态机持有，状态自身不再存储转移。
    /// </summary>
    public abstract class StateBase<T> : IState<T>
    {
        protected string name;
        protected readonly Guid id;
        protected bool enabled;
        protected T context;

        public StateBase(string name, bool enabled = true)
        {
            this.name = name ?? throw new ArgumentNullException(nameof(name));
            this.id = Guid.NewGuid();
            this.enabled = enabled;
        }

        public virtual string Name => name;
        public Guid Id => id;
        public virtual bool Enabled { get => enabled; set => enabled = value; }
        public virtual T Context { get => context; set => context = value; }

        public abstract void OnEnter();
        public abstract void OnUpdate(float deltaTime);
        public abstract void OnExit();
    }
}
