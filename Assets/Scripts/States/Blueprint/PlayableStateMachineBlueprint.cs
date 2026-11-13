using System;

namespace MNAC.States
{
    /// <summary>非泛型薄壳，等价于 <see cref="PlayableStateMachineBlueprint{T}"/> with T=object。</summary>
    public class PlayableStateMachineBlueprint : PlayableStateMachineBlueprint<object>
    {
        public PlayableStateMachineBlueprint(string name) : base(name)
        {
        }
    }

    /// <summary>
    /// 可播放状态机的图描述。Build 产出 <see cref="PlayableStateMachine{T}"/>，
    /// 每条转移都会创建独立的 <see cref="PlayableTransition{T}"/>（含独立 Timeline）。
    /// </summary>
    public class PlayableStateMachineBlueprint<T>
        : StateMachineBlueprintBase<T, PlayableStateMachine<T>, PlayableStateMachineBlueprint<T>>
    {
        public PlayableStateMachineBlueprint(string name) : base(name)
        {
        }

        protected override PlayableStateMachine<T> CreateMachine() => new PlayableStateMachine<T>(Name);

        /// <summary>注册一条带时长 / 打断模式的 Playable 转移。</summary>
        public PlayableStateMachineBlueprint<T> PlayableTransition(
            string from,
            string to,
            float duration = 0f,
            ITransitionCondition<T> condition = null,
            Action<IPlayableState<T>, IPlayableState<T>, float> durationEvent = null,
            InterruptionMode interruptionMode = PlayableTransition<T>.INTERRUPTION_MODE_DEFAULT,
            Action<T> onTriggered = null)
        {
            AddTransitionSpec(new TransitionSpec<T>(from, to, condition)
            {
                Duration = duration,
                DurationEvent = durationEvent,
                InterruptionMode = interruptionMode,
                OnTriggered = onTriggered,
            });
            return this;
        }

        /// <summary>注册一条用 lambda 描述条件的 Playable 转移。</summary>
        public PlayableStateMachineBlueprint<T> PlayableTransition(
            string from,
            string to,
            float duration,
            Func<T, bool> condition,
            Action<IPlayableState<T>, IPlayableState<T>, float> durationEvent = null,
            InterruptionMode interruptionMode = PlayableTransition<T>.INTERRUPTION_MODE_DEFAULT,
            Action<T> onTriggered = null)
        {
            if (condition == null)
                throw new ArgumentNullException(nameof(condition));
            return PlayableTransition(
                from,
                to,
                duration,
                new DelegateTransitionCondition<T>(condition),
                durationEvent,
                interruptionMode,
                onTriggered);
        }

        protected override Transition<T> CreateTransition(
            PlayableStateMachine<T> machine,
            IState<T> source,
            IState<T> destination,
            TransitionSpec<T> spec)
        {
            if (source is not IPlayableState<T> playableSource || destination is not IPlayableState<T> playableDestination)
                throw new InvalidCastException(
                    $"PlayableStateMachineBlueprint requires IPlayableState<T> states, but got " +
                    $"'{source.GetType().Name}' -> '{destination.GetType().Name}'.");

            return new PlayableTransition<T>(
                playableSource,
                playableDestination,
                null,
                spec.DurationEvent,
                spec.Duration,
                spec.InterruptionMode);
        }
    }
}
