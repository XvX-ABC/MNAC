using MNAC.Utilities.Timeline;

namespace MNAC.StatesNew
{
    /// <summary>
    /// 可播放状态：带一条 Timeline，并提供六个过渡回调，
    /// 让状态感知"我从哪个状态过渡而来 / 正在过渡去哪个状态"。
    /// </summary>
    public interface IPlayableState<T> : IState<T>
    {
        ITimeline Timeline { get; }

        void FromPreviousStateTransitionBegin(IReadonlyPlayableTransition<T> currentTransition);
        void FromPreviousStateTransitionRunning(IReadonlyPlayableTransition<T> currentTransition);
        void FromPreviousStateTransitionEnd(IReadonlyPlayableTransition<T> currentTransition);

        void ToNextStateTransitionBegin(IReadonlyPlayableTransition<T> currentTransition);
        void ToNextStateTransitionRunning(IReadonlyPlayableTransition<T> currentTransition);
        void ToNextStateTransitionEnd(IReadonlyPlayableTransition<T> currentTransition);
    }
}
