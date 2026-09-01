namespace MNAC.StatesNew
{
    /// <summary>过渡打断模式（旧库名 InterruptionSource，此处统一命名）。</summary>
    public enum InterruptionMode
    {
        /// <summary>过渡播放期间不允许被目标状态的出边打断。</summary>
        None,

        /// <summary>过渡播放期间，目标状态的出边（带触发条件者）可作为中断源，直接改道新目标。</summary>
        Next,
    }
}
