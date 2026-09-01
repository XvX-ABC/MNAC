using System;

namespace MNAC.StatesNew
{
    /// <summary>带回调的可播放状态：生命周期用委托驱动，外部无需子类化即可挂逻辑。</summary>
    public interface IWithCallbackPlayableState<T> : IPlayableState<T>
    {
        Action EntryAction { get; set; }
        Action UpdateAction { get; set; }
        Action ExitAction { get; set; }
    }
}
