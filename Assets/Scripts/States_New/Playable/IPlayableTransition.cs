using MNAC.Utilities.Timeline;

namespace MNAC.StatesNew
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
}
