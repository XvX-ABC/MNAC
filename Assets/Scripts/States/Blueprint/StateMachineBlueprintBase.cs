using System;
using System.Collections.Generic;

namespace MNAC.States
{
    /// <summary>
    /// 状态机图描述基类：只保存 key 与工厂，不保存任何运行时实例。
    /// 每次 <see cref="Build"/> 都重新创建状态、转移与 Timeline，实例之间完全隔离。
    ///
    /// 配置完成后应视为只读；Build 不是线程安全的。
    /// </summary>
    public abstract class StateMachineBlueprintBase<T, TMachine, TSelf>
        where TMachine : StateMachine<T>
        where TSelf : StateMachineBlueprintBase<T, TMachine, TSelf>
    {
        readonly Dictionary<string, Func<IState<T>>> stateFactories = new();
        readonly List<string> stateOrder = new();
        readonly List<TransitionSpec<T>> transitionSpecs = new();

        protected StateMachineBlueprintBase(string name)
        {
            Name = string.IsNullOrEmpty(name)
                ? throw new ArgumentException("Blueprint name must be non-empty.", nameof(name))
                : name;
        }

        public string Name { get; }

        public IReadOnlyDictionary<string, Func<IState<T>>> StateFactories => stateFactories;

        public IReadOnlyList<TransitionSpec<T>> TransitionSpecs => transitionSpecs;

        /// <summary>初始状态 key；未设置时取首个注册状态。</summary>
        public string EntryKey { get; private set; }

        /// <summary>注册一个状态工厂。同一个 key 只能注册一次。</summary>
        public TSelf State(string key, Func<IState<T>> factory)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("State key must be non-empty.", nameof(key));
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));
            if (stateFactories.ContainsKey(key))
                throw new DuplicateStateKeyException(key);

            stateFactories.Add(key, factory);
            stateOrder.Add(key);
            return (TSelf)this;
        }

        /// <summary>注册一个无参构造的状态类型。</summary>
        public TSelf State<TState>(string key) where TState : IState<T>, new()
        {
            return State(key, () => new TState());
        }

        /// <summary>注册一条带上下文条件的转移。</summary>
        public TSelf Transition(
            string from,
            string to,
            ITransitionCondition<T> condition = null,
            Action<T> onTriggered = null)
        {
            AddTransitionSpec(new TransitionSpec<T>(from, to, condition)
            {
                OnTriggered = onTriggered,
            });
            return (TSelf)this;
        }

        /// <summary>注册一条用 lambda 描述的条件转移。</summary>
        public TSelf Transition(
            string from,
            string to,
            Func<T, bool> condition,
            Action<T> onTriggered = null)
        {
            if (condition == null)
                throw new ArgumentNullException(nameof(condition));
            return Transition(from, to, new DelegateTransitionCondition<T>(condition), onTriggered);
        }

        /// <summary>设置初始状态 key。</summary>
        public TSelf Entry(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Entry key must be non-empty.", nameof(key));
            EntryKey = key;
            return (TSelf)this;
        }

        /// <summary>
        /// 按当前描述构建一台全新状态机。所有工厂都会被重新执行。
        /// </summary>
        public TMachine Build(T context)
        {
            var machine = CreateMachine()
                ?? throw new StateMachineException($"Blueprint '{Name}' returned a null machine.");

            // 先注入 Context，AddState 会把它广播给每个状态。
            machine.Context = context;

            foreach (var key in stateOrder)
            {
                var state = stateFactories[key]();
                if (state == null)
                    throw new StateMachineException(
                        $"State factory for key '{key}' in blueprint '{Name}' returned null.");
                machine.AddState(key, state);
            }

            foreach (var spec in transitionSpecs)
            {
                var source = machine.GetState(spec.SourceKey);
                var destination = machine.GetState(spec.DestinationKey);
                var transition = CreateTransition(machine, source, destination, spec);
                transition.OnTriggered = spec.OnTriggered;
                if (spec.Condition != null)
                    transition.AddCondition(spec.Condition);
                machine.AddTransition(transition);
            }

            if (!string.IsNullOrEmpty(EntryKey))
                machine.SetInitialState(machine.GetState(EntryKey));

            return machine;
        }

        /// <summary>创建目标机器类型；子类返回 core 或 Playable 状态机。</summary>
        protected abstract TMachine CreateMachine();

        /// <summary>创建一条转移；Playable 子类重写为 <see cref="PlayableTransition{T}"/>。</summary>
        protected virtual Transition<T> CreateTransition(
            TMachine machine,
            IState<T> source,
            IState<T> destination,
            TransitionSpec<T> spec)
        {
            return new Transition<T>(source, destination);
        }

        /// <summary>注册一条已构造的转移描述，供子类扩展 Playable 字段。</summary>
        protected void AddTransitionSpec(TransitionSpec<T> spec)
        {
            if (spec == null)
                throw new ArgumentNullException(nameof(spec));
            transitionSpecs.Add(spec);
        }
    }
}
