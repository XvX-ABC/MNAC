using System;

namespace MNAC.States
{
    /// <summary>
    /// Blueprint 中的一条转移描述。core 层只使用 key 与 condition；
    /// Playable 层额外使用 duration / interruptionMode / durationEvent。
    /// </summary>
    public sealed class TransitionSpec<T>
    {
        public TransitionSpec(string sourceKey, string destinationKey, ITransitionCondition<T> condition = null)
        {
            SourceKey = string.IsNullOrEmpty(sourceKey)
                ? throw new ArgumentException("Source key must be non-empty.", nameof(sourceKey))
                : sourceKey;
            DestinationKey = string.IsNullOrEmpty(destinationKey)
                ? throw new ArgumentException("Destination key must be non-empty.", nameof(destinationKey))
                : destinationKey;
            Condition = condition;
        }

        public string SourceKey { get; }
        public string DestinationKey { get; }
        public ITransitionCondition<T> Condition { get; internal set; }

        /// <summary>Playable 过渡时长；core Blueprint 忽略。</summary>
        public float Duration { get; internal set; }

        /// <summary>Playable 过渡打断模式；core Blueprint 忽略。</summary>
        public InterruptionMode InterruptionMode { get; internal set; }

        /// <summary>Playable 逐帧混合回调；core Blueprint 忽略。</summary>
        public Action<IPlayableState<T>, IPlayableState<T>, float> DurationEvent { get; internal set; }

        /// <summary>转移触发时的回调（在切换前执行）。</summary>
        public Action<T> OnTriggered { get; internal set; }
    }
}
