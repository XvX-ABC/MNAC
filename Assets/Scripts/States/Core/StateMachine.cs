using System;
using System.Collections.Generic;

namespace MNAC.States
{
    /// <summary>非泛型薄壳，等价于 <see cref="StateMachine{T}"/> with T=object。</summary>
    public class StateMachine : StateMachine<object>
    {
        public StateMachine(string name, bool enabled = true) : base(name, enabled)
        {
        }
    }

    /// <summary>
    /// 状态机核心。纯逻辑、零 Timeline 依赖（Playable 层在其上扩展）。
    ///
    /// 设计要点：
    /// - 转移表由状态机持有（<see cref="transitions"/>），状态自身不存转移——RemoveState 可清理全部关联边，无双份缓存。
    /// - 初始状态显式设置（<see cref="SetInitialState"/>），未设置则取首个 AddState 的状态。
    /// - 自转移（源==目标）统一 no-op，不触发切换。
    /// - 生命周期异常用 <see cref="StateExitException"/>/<see cref="StateEnterException"/>/<see cref="StateUpdateException"/> 包装并携带状态名。
    /// - <see cref="Update(float)"/> 显式传 deltaTime，可测、可支持 FixedUpdate。
    ///
    /// 本类同时实现 <see cref="IState{T}"/>：作为状态被外层机器 AddState 嵌套，
    /// OnEnter 进入本机初始态、OnUpdate 推进本机、OnExit 退出当前子状态。
    /// </summary>
    public class StateMachine<T> : IState<T>, IStateMachineInspection
    {
        protected readonly List<IState<T>> states;
        protected readonly List<Transition<T>> transitions;
        protected readonly string name;
        protected readonly Guid id;
        protected bool enabled;
        protected T context;
        protected IState<T> current;
        protected IState<T> initial;

        public StateMachine(string name, bool enabled = true)
        {
            this.name = name ?? throw new ArgumentNullException(nameof(name));
            this.id = Guid.NewGuid();
            this.enabled = enabled;
            states = new List<IState<T>>();
            transitions = new List<Transition<T>>();
        }

        public virtual string Name => name;
        public Guid Id => id;
        public virtual bool Enabled { get => enabled; set => enabled = value; }
        public IState<T> CurrentState => current;

        /// <summary>只读状态列表（调试/工具用，运行时不要修改返回的列表）。</summary>
        public IReadOnlyList<IState<T>> States => states;

        /// <summary>只读转移列表（调试/工具用，运行时不要修改返回的列表）。</summary>
        public IReadOnlyList<Transition<T>> Transitions => transitions;

        /// <summary>
        /// 共享上下文。赋值会广播给所有已注册状态（O(状态数)），建议在 AddState 前或 setup 期设置。
        /// </summary>
        public virtual T Context
        {
            get => context;
            set
            {
                context = value;
                foreach (var state in states)
                    state.Context = value;
            }
        }

        // ---------- 状态注册 ----------

        /// <summary>注册状态并注入当前上下文。重复注册 no-op。</summary>
        public void AddState(IState<T> state)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state));
            if (states.Contains(state))
                return;
            state.Context = context;
            states.Add(state);
        }

        /// <summary>移除状态，并清理所有以它为源或目标的转移。</summary>
        public bool RemoveState(IState<T> state)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state));
            if (!states.Contains(state))
                return false;
            if (current == state)
            {
                try
                {
                    state.OnExit();
                }
                catch (Exception e)
                {
                    throw new StateExitException(state.Name, e);
                }
                if (current == state)
                    current = null; // OnExit 内可能已经切到别的状态，不要覆盖
            }
            states.Remove(state);
            state.Context = default;
            transitions.RemoveAll(t => t.Source == state || t.Destination == state);
            if (initial == state)
                initial = null;
            return true;
        }

        public bool Contains(IState<T> state) => state != null && states.Contains(state);

        /// <summary>显式设置初始状态（首个 Update 时进入）。未设置则取首个 AddState 的状态。</summary>
        public void SetInitialState(IState<T> state)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state));
            if (!states.Contains(state))
                throw new StateNotRegisteredException(state.Name);
            initial = state;
        }

        // ---------- 转移注册 ----------

        /// <summary>创建并注册一条转移。source/destination 必须已注册。</summary>
        public Transition<T> AddTransition(IState<T> source, IState<T> destination, Func<bool> trigger = null, Action<T> onTriggered = null)
        {
            var transition = CreateTransition(source, destination, trigger);
            transition.OnTriggered = onTriggered;
            RegisterTransition(transition);
            return transition;
        }

        /// <summary>子类可重写以创建特定类型的转移（如 Playable 层的 PlayableTransition）。</summary>
        protected virtual Transition<T> CreateTransition(IState<T> source, IState<T> destination, Func<bool> trigger)
        {
            return new Transition<T>(source, destination, trigger);
        }

        /// <summary>注册已构造的转移（Playable 层构造完成后调用）。</summary>
        protected void RegisterTransition(Transition<T> transition)
        {
            if (transition == null)
                throw new ArgumentNullException(nameof(transition));
            if (!states.Contains(transition.Source))
                throw new StateNotRegisteredException(transition.Source.Name);
            if (!states.Contains(transition.Destination))
                throw new StateNotRegisteredException(transition.Destination.Name);
            transitions.Add(transition);
        }

        public bool RemoveTransition(Transition<T> transition) => transition != null && transitions.Remove(transition);

        public void ClearTransitionsFrom(IState<T> source) => transitions.RemoveAll(t => t.Source == source);

        // ---------- 状态切换 ----------

        /// <summary>显式切换到某状态；已在当前状态则 no-op。</summary>
        public void ChangeStateTo(IState<T> state) => ChangeState(state);

        protected virtual void ChangeState(IState<T> next)
        {
            if (next == null)
                throw new ArgumentNullException(nameof(next));
            if (!states.Contains(next))
                throw new StateNotRegisteredException(next.Name);
            SwitchTo(next);
        }

        /// <summary>内部切换：不做注册校验，供 Playable 过渡（TransitionState 非注册状态）使用。</summary>
        protected void SwitchTo(IState<T> next)
        {
            if (next == null)
                throw new ArgumentNullException(nameof(next));
            if (next == current)
                return;
            var previous = current;
            if (previous != null)
            {
                try
                {
                    previous.OnExit();
                }
                catch (Exception e)
                {
                    throw new StateExitException(previous.Name, e);
                }
            }
            // 先提交 current 再 Enter：OnEnter 内可以安全读取 CurrentState；
            // 若 Enter 失败则 current 置空，避免机器指向一个已经 Exit、却没有 Enter 成功的状态。
            current = next;
            try
            {
                next.OnEnter();
            }
            catch (Exception e)
            {
                current = null;
                throw new StateEnterException(next.Name, e);
            }
        }

        // ---------- 每帧驱动 ----------

        public virtual void Update(float deltaTime)
        {
            if (!enabled)
                return;
            if (current == null)
            {
                EnterInitialState();
                if (current == null)
                    return;
            }
            Transition<T> transition;
            try
            {
                transition = Evaluate(current);
            }
            catch (Exception e)
            {
                throw new StateTransitionException(current?.Name, e);
            }
            if (transition != null)
                ApplyTransition(transition);
            var updating = current;
            try
            {
                updating?.OnUpdate(deltaTime);
            }
            catch (Exception e)
            {
                throw new StateUpdateException(updating?.Name, e);
            }
        }

        protected virtual void EnterInitialState()
        {
            if (initial != null)
            {
                ChangeState(initial);
                return;
            }
            if (states.Count > 0)
                ChangeState(states[0]);
        }

        /// <summary>返回当前状态的第一条满足条件的转移（目标需 Enabled），按注册顺序。</summary>
        protected virtual Transition<T> Evaluate(IState<T> state)
        {
            foreach (var transition in transitions)
            {
                if (transition.Source != state)
                    continue;
                if (!transition.Destination.Enabled)
                    continue;
                if (transition.IsTriggered)
                    return transition;
            }
            return null;
        }

        /// <summary>应用转移。默认直切；Playable 子类重写为走过渡。</summary>
        protected virtual void ApplyTransition(Transition<T> transition)
        {
            if (transition.Destination == current)
            {
                InvokeTriggered(transition);
                return;
            }
            InvokeTriggered(transition);
            ChangeState(transition.Destination);
        }

        /// <summary>执行 OnTriggered 回调，并把回调异常包装为转移异常。</summary>
        protected void InvokeTriggered(Transition<T> transition)
        {
            try
            {
                transition.OnTriggered?.Invoke(context);
            }
            catch (Exception e)
            {
                throw new StateTransitionException(transition.Source.Name, e);
            }
        }

        // ---------- 作为状态（嵌套） ----------

        /// <summary>作为状态被外层机器进入：首次确定性进入初始态，再次进入则转发给当前子状态。</summary>
        public virtual void OnEnter()
        {
            if (current == null)
            {
                EnterInitialState();
                return;
            }
            try
            {
                current.OnEnter();
            }
            catch (Exception e)
            {
                throw new StateEnterException(current.Name, e);
            }
        }

        /// <summary>作为状态时每帧推进（等价于 Update）。</summary>
        public virtual void OnUpdate(float deltaTime) => Update(deltaTime);

        /// <summary>作为状态退出：退出当前子状态并清空，下次进入重新从初始态开始。</summary>
        public virtual void OnExit()
        {
            if (current == null)
                return;
            try
            {
                current.OnExit();
            }
            catch (Exception e)
            {
                throw new StateExitException(current.Name, e);
            }
            current = null;
        }

        public override string ToString()
        {
            return current == null
                ? $"StateMachine '{name}' (no current state)"
                : $"StateMachine '{name}' -> '{current.Name}'";
        }

        // ---------- 只读快照（Inspector / 调试工具） ----------

        /// <summary>当前正在播放的过渡；基类没有过渡概念，默认 null。</summary>
        protected virtual Transition<T> ActiveTransition => null;

        bool IStateMachineInspection.Enabled => enabled;
        string IStateMachineInspection.CurrentStateName => current?.Name;
        bool IStateMachineInspection.IsTransitioning => ActiveTransition != null;

        string IStateMachineInspection.ActiveTransitionSource => ActiveTransition?.Source?.Name;
        string IStateMachineInspection.ActiveTransitionDestination => ActiveTransition?.Destination?.Name;

        float IStateMachineInspection.ActiveTransitionProgress
        {
            get
            {
                if (ActiveTransition is IPlayableTransition<T> playable && playable.Timeline != null)
                    return playable.Timeline.NormalizedTime;
                return 0f;
            }
        }

        IReadOnlyList<StateSnapshot> IStateMachineInspection.StateSnapshots => CreateStateSnapshots();
        IReadOnlyList<TransitionSnapshot> IStateMachineInspection.TransitionSnapshots => CreateTransitionSnapshots();

        List<StateSnapshot> CreateStateSnapshots()
        {
            var result = new List<StateSnapshot>(states.Count);
            foreach (var state in states)
                result.Add(new StateSnapshot(state.Name, state.GetType().Name, state.Enabled, state == current));
            return result;
        }

        List<TransitionSnapshot> CreateTransitionSnapshots()
        {
            var active = ActiveTransition;
            var result = new List<TransitionSnapshot>(transitions.Count);
            foreach (var transition in transitions)
            {
                var playable = transition as IPlayableTransition<T>;
                var timeline = playable?.Timeline;
                var isActive = ReferenceEquals(transition, active);
                result.Add(new TransitionSnapshot(
                    transition.Source.Name,
                    transition.Destination.Name,
                    timeline?.Length ?? 0f,
                    playable?.InterruptionMode ?? InterruptionMode.None,
                    transition.Triggers.Count,
                    transition.HasTriggerResult,
                    transition.LastTriggerResult,
                    isActive,
                    isActive ? timeline?.NormalizedTime ?? 0f : 0f));
            }
            return result;
        }
    }
}
