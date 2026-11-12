using MNAC.Utilities.Timeline;

namespace MNAC.States
{
    /// <summary>可播放转移的只读视图（供过渡回调参数使用）。</summary>
    public interface IReadonlyPlayableTransition<T>
    {
        ITimeline Timeline { get; }
        InterruptionMode InterruptionMode { get; }
    }

    /// <summary>
    /// 可播放转移接口：PlayableStateMachine 据此识别走过渡路径，
    /// 而非 <see cref="Transition{T}"/> 的直切路径。
    /// Source/Destination 由 <see cref="PlayableTransition{T}"/> 从 <see cref="Transition{T}"/> 继承实现。
    /// </summary>
    public interface IPlayableTransition<T> : IReadonlyPlayableTransition<T>
    {
        IState<T> Source { get; }
        IState<T> Destination { get; }
    }

    /// <summary>可被中断的转移：中断时需要清理自己申请的临时状态（如 Blending 缩短的目标 Timeline）。</summary>
    public interface IInterruptiblePlayableTransition<T> : IPlayableTransition<T>
    {
        void OnInterrupted();
    }
}
