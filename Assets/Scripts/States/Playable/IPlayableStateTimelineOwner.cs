namespace MNAC.States
{
    /// <summary>
    /// 包装型状态的 Timeline 归属：外层做 Blending 时应缩短/订阅真正的子状态，
    /// 而不是包装层自己那条独立 Timeline。
    /// </summary>
    public interface IPlayableStateTimelineOwner<T>
    {
        IWithCallbackPlayableState<T> TimelineOwner { get; }
    }
}
