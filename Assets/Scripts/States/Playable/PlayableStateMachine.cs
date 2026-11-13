using System;
using System.Collections.Generic;
using MNAC.Utilities.Timeline;

namespace MNAC.States
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

            /// <summary>结束当前过渡 Timeline（触发 End 回调并暂停），用于中断/外部切换。</summary>
            public void EndCurrent()
            {
                if (timeline != null && timeline.IsRunning)
                    timeline.End();
            }

            /// <summary>中断当前过渡：先清理转移自己申请的临时状态，再结束 Timeline。</summary>
            public void InterruptCurrent()
            {
                if (!IsComplete && currentTransition is IInterruptiblePlayableTransition<T> interruptible)
                    interruptible.OnInterrupted();
                EndCurrent();
            }

            /// <summary>用新转移重配置。中断旧 Timeline；重启新 Timeline。</summary>
            public void Configure(PlayableStateMachine<T> owner, IPlayableTransition<T> transition)
            {
                InterruptCurrent(); // 中断旧过渡时也要清理/结束，避免 Timeline 永远停在运行中
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
                    if (t.Source == destination && t.HasTriggerConditions)
                        interruptTransitions.Add(t);
                }
            }

            /// <summary>返回第一条满足条件的中断转移（目标需 Enabled）。</summary>
            public Transition<T> CheckInterruption(T context)
            {
                foreach (var t in interruptTransitions)
                {
                    try
                    {
                        if (t.Destination.Enabled && t.Evaluate(context))
                            return t;
                    }
                    catch (Exception e)
                    {
                        throw new StateTransitionException(t.Source.Name, e);
                    }
                }
                return null;
            }

            // Configure 已负责 Restart；这里再 Restart 会重复重置事件游标。
            public override void OnEnter()
            {
            }

            public override void OnUpdate(float deltaTime)
            {
                timeline?.OnUpdate(deltaTime);
                // Timeline 的 End 发生在其时间已经达到 length 的“下一次” OnUpdate；
                // 而 IsComplete 用的是 NormalizedTime，会在同一帧先变 true。
                // 这里补一拍把 End 冲出来，确保过渡的 End 回调不会被外层状态切换吞掉。
                if (timeline != null && timeline.IsRunning && timeline.NormalizedTime >= 1f)
                    timeline.OnUpdate(0f);
            }

            // 外部切换离开过渡状态时也要结束 Timeline；完成路径此时已 Pause，不会二次触发。
            public override void OnExit()
            {
                if (!IsComplete)
                    InterruptCurrent();
                else
                    EndCurrent();
            }
        }

        readonly TransitionState transitionState;

        public PlayableStateMachine(string name, bool enabled = true) : base(name, enabled)
        {
            transitionState = new TransitionState();
        }

        /// <summary>当前正在播放的过渡；非过渡状态时为 null。只读，供 Inspector 展示。</summary>
        protected override Transition<T> ActiveTransition =>
            (current as TransitionState)?.CurrentTransition as Transition<T>;

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
                InvokeTriggered(transition);
                return;
            }
            InvokeTriggered(transition);
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
                AdvanceTransition(ts, deltaTime);
                return;
            }

            base.Update(deltaTime);

            // base.Update 可能在本帧刚进入过渡状态（且已经推进过一次 delta）。
            // 这里只处理“零时长过渡”的同帧完成，不再重复推进 delta。
            if (current is TransitionState started)
                CompleteTransitionIfDone(started);
        }

        void AdvanceTransition(TransitionState ts, float deltaTime)
        {
            try
            {
                ts.OnUpdate(deltaTime);
            }
            catch (Exception e)
            {
                throw new StateUpdateException(ts.Name, e);
            }

            if (ts.IsComplete)
            {
                CompleteTransitionIfDone(ts);
                return;
            }

            if (ts.CheckInterruption(context) is Transition<T> interruption)
                ApplyInterruption(ts, interruption);
        }

        void CompleteTransitionIfDone(TransitionState ts)
        {
            if (!ts.IsComplete)
                return;
            var destination = ts.CurrentTransition.Destination;
            ChangeState(destination);
            // 过渡已经消耗了本帧 delta，目标状态只做 0 时刻的进入回调，避免时间重复推进。
            UpdateCurrent(0f);
        }

        void UpdateCurrent(float deltaTime)
        {
            var target = current;
            try
            {
                target?.OnUpdate(deltaTime);
            }
            catch (Exception e)
            {
                throw new StateUpdateException(target?.Name, e);
            }
        }

        void ApplyInterruption(TransitionState ts, Transition<T> interruption)
        {
            InvokeTriggered(interruption);
            if (interruption is IPlayableTransition<T> playable && playable.Timeline != null)
            {
                ts.Configure(this, playable);   // 结束旧 Timeline、重启新 Timeline
                try
                {
                    ts.OnUpdate(0f);
                }
                catch (Exception e)
                {
                    throw new StateUpdateException(ts.Name, e);
                }
                CompleteTransitionIfDone(ts);
            }
            else
            {
                ts.InterruptCurrent();          // 直切前清理/结束旧 Timeline
                ChangeState(interruption.Destination);
                UpdateCurrent(0f);
            }
        }
    }
}
