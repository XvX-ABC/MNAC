using System;
using System.Collections.Generic;
using MNAC.Utilities.Timeline;

namespace MNAC.StatesNew
{
    /// <summary>非泛型薄壳，等价于 <see cref="PlayableStateMachine{T}"/> with T=object。</summary>
    public class PlayableStateMachine : PlayableStateMachine<object>
    {
        public PlayableStateMachine(string name, bool enabled = true) : base(name, enabled)
        {
        }
    }

    /// <summary>
    /// 可播放状态机：切换不是瞬间完成，而是先进入一个"过渡状态"播放转移的 Timeline，
    /// 播完才真正进入目标状态。过渡期间可被目标状态的出边打断（<see cref="InterruptionMode.Next"/>）。
    ///
    /// 相比旧库：中断不再浅拷贝数组，改为在机器转移表上按 source==目标 查询；
    /// 过渡状态 OnExit 故意 no-op，避免 End 事件二次触发。
    /// </summary>
    public class PlayableStateMachine<T> : StateMachine<T>, IPlayableState<T>
    {
        public const InterruptionMode INTERRUPTION_MODE_DEFAULT = PlayableTransition<T>.INTERRUPTION_MODE_DEFAULT;

        /// <summary>
        /// 过渡状态：把"切换"本身做成一个临时状态，播放转移的 Timeline。
        /// </summary>
        class TransitionState : PlayableStateBase<T>
        {
            readonly List<Transition<T>> interruptTransitions = new();
            IPlayableTransition<T> currentTransition;

            public TransitionState() : base("", 0, true)
            {
            }

            public IPlayableTransition<T> CurrentTransition => currentTransition;

            /// <summary>Timeline 播完即完成。</summary>
            public bool IsComplete => timeline != null && timeline.NormalizedTime >= 1f;

            /// <summary>用新转移重配置。重启 Timeline；中断时原地重指向，不重进 OnEnter。</summary>
            public void Configure(PlayableStateMachine<T> owner, IPlayableTransition<T> transition)
            {
                currentTransition = transition;
                timeline = transition.Timeline;
                name = $"{transition.Source.Name} -> {transition.Destination.Name}";
                UpdateInterrupts(owner);
                timeline?.Restart();
            }

            void UpdateInterrupts(PlayableStateMachine<T> owner)
            {
                interruptTransitions.Clear();
                if (currentTransition.InterruptionMode != InterruptionMode.Next)
                    return;
                var destination = currentTransition.Destination;
                foreach (var t in owner.transitions)
                {
                    // 只取目标状态上带触发条件的出边，避免无条件转移立即打断当前过渡
                    if (t.Source == destination && t.Triggers.Count > 0)
                        interruptTransitions.Add(t);
                }
            }

            /// <summary>返回第一条满足条件的中断转移（目标需 Enabled）。</summary>
            public Transition<T> CheckInterruption()
            {
                foreach (var t in interruptTransitions)
                {
                    if (t.Destination.Enabled && t.IsTriggered)
                        return t;
                }
                return null;
            }

            public override void OnEnter() => timeline?.Restart();

            public override void OnUpdate(float deltaTime) => timeline?.OnUpdate(deltaTime);

            // 故意 no-op：End 事件由 Timeline 播完时触发，这里再 End 会二次触发
            public override void OnExit()
            {
            }
        }

        readonly TransitionState transitionState;

        public PlayableStateMachine(string name, bool enabled = true) : base(name, enabled)
        {
            transitionState = new TransitionState();
        }

        // 让继承自 Core 的 AddTransition(source, dest, trigger) 也产出可播放转移（零时长）
        protected override Transition<T> CreateTransition(IState<T> source, IState<T> destination, Func<bool> trigger)
        {
            if (source is not IPlayableState<T> ps || destination is not IPlayableState<T> pd)
                throw new InvalidCastException(
                    $"PlayableStateMachine requires IPlayableState<T> states, but got '{source.GetType().Name}' -> '{destination.GetType().Name}'.");
            return new PlayableTransition<T>(ps, pd, trigger);
        }

        /// <summary>完整版添加转移：时长 + 逐帧混合回调 + 打断模式。</summary>
        public PlayableTransition<T> AddTransition(
            IPlayableState<T> source,
            IPlayableState<T> destination,
            float duration = 0,
            Func<bool> trigger = null,
            Action<IPlayableState<T>, IPlayableState<T>, float> durationEvent = null,
            InterruptionMode interruptionMode = INTERRUPTION_MODE_DEFAULT)
        {
            var transition = new PlayableTransition<T>(source, destination, trigger, durationEvent, duration, interruptionMode);
            RegisterTransition(transition);
            return transition;
        }

        // ---------- 作为 IPlayableState（机器可被外层机器当状态用） ----------

        public virtual ITimeline Timeline => (current as IPlayableState<T>)?.Timeline;

        public virtual void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<T> currentTransition) => (current as IPlayableState<T>)?.FromPreviousStateTransitionBegin(currentTransition);
        public virtual void FromPreviousStateTransitionRunning(IReadonlyPlayableTransition<T> currentTransition) => (current as IPlayableState<T>)?.FromPreviousStateTransitionRunning(currentTransition);
        public virtual void FromPreviousStateTransitionEnd(IReadonlyPlayableTransition<T> currentTransition) => (current as IPlayableState<T>)?.FromPreviousStateTransitionEnd(currentTransition);
        public virtual void ToNextStateTransitionBegin(IReadonlyPlayableTransition<T> currentTransition) => (current as IPlayableState<T>)?.ToNextStateTransitionBegin(currentTransition);
        public virtual void ToNextStateTransitionRunning(IReadonlyPlayableTransition<T> currentTransition) => (current as IPlayableState<T>)?.ToNextStateTransitionRunning(currentTransition);
        public virtual void ToNextStateTransitionEnd(IReadonlyPlayableTransition<T> currentTransition) => (current as IPlayableState<T>)?.ToNextStateTransitionEnd(currentTransition);

        // ---------- 过渡路径 ----------

        protected override void ApplyTransition(Transition<T> transition)
        {
            if (transition.Destination == current)
            {
                transition.OnTriggered?.Invoke(context);
                return;
            }
            transition.OnTriggered?.Invoke(context);
            if (transition is IPlayableTransition<T> playable && playable.Timeline != null)
            {
                transitionState.Configure(this, playable);
                SwitchTo(transitionState);
            }
            else
            {
                ChangeState(transition.Destination);   // 直切
            }
        }

        public override void Update(float deltaTime)
        {
            if (!enabled)
                return;
            if (current is TransitionState ts)
            {
                ts.OnUpdate(deltaTime);
                if (ts.IsComplete)
                {
                    // 完成优先于中断：过渡播完直接进入目标
                    ChangeState(ts.CurrentTransition.Destination);
                    current?.OnUpdate(deltaTime);
                }
                else if (ts.CheckInterruption() is Transition<T> interruption)
                {
                    ApplyInterruption(ts, interruption);
                }
                return;
            }
            base.Update(deltaTime);
        }

        void ApplyInterruption(TransitionState ts, Transition<T> interruption)
        {
            interruption.OnTriggered?.Invoke(context);
            if (interruption is IPlayableTransition<T> playable && playable.Timeline != null)
            {
                ts.Configure(this, playable);   // 原地重指向，新 Timeline 已 Restart
            }
            else
            {
                ChangeState(interruption.Destination);   // 直切
            }
        }
    }
}
