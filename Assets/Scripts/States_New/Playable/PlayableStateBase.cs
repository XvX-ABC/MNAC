using MNAC.Utilities.Timeline;

namespace MNAC.StatesNew
{
    /// <summary>非泛型薄壳，等价于 <see cref="PlayableStateBase{T}"/> with T=object。</summary>
    public abstract class PlayableStateBase : PlayableStateBase<object>
    {
        public PlayableStateBase(string name, float duration = 0, bool enabled = true) : base(name, duration, enabled)
        {
        }
    }

    /// <summary>
    /// 可播放状态基类。默认生命周期自动驱动 Timeline：
    /// OnEnter=Restart、OnUpdate=推进、OnExit=若仍在播放则 End。
    /// 子类可按需 override（并调用 base）或完全接管。
    /// </summary>
    public abstract class PlayableStateBase<T> : StateBase<T>, IPlayableState<T>
    {
        protected ITimeline timeline;

        public PlayableStateBase(string name, float duration = 0, bool enabled = true) : base(name, enabled)
        {
            timeline = NewTimeline(duration);
        }

        /// <summary>创建 Timeline 的钩子，子类可返回 null（如过渡状态）或自定义实现。</summary>
        protected virtual ITimeline NewTimeline(float duration)
        {
            return new Timeline(duration);
        }

        public ITimeline Timeline => timeline;

        public override void OnEnter()
        {
            timeline?.Restart();
        }

        public override void OnUpdate(float deltaTime)
        {
            timeline?.OnUpdate(deltaTime);
        }

        public override void OnExit()
        {
            // 若已播完（IsRunning=false）则不再 End，避免二次触发 EndAction
            if (timeline != null && timeline.IsRunning)
                timeline.End();
        }

        public virtual void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<T> currentTransition)
        {
        }

        public virtual void FromPreviousStateTransitionRunning(IReadonlyPlayableTransition<T> currentTransition)
        {
        }

        public virtual void FromPreviousStateTransitionEnd(IReadonlyPlayableTransition<T> currentTransition)
        {
        }

        public virtual void ToNextStateTransitionBegin(IReadonlyPlayableTransition<T> currentTransition)
        {
        }

        public virtual void ToNextStateTransitionRunning(IReadonlyPlayableTransition<T> currentTransition)
        {
        }

        public virtual void ToNextStateTransitionEnd(IReadonlyPlayableTransition<T> currentTransition)
        {
        }
    }
}
