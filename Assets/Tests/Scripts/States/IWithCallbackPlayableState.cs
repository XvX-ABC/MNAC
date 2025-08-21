using System;
namespace Tests.States
{
    public interface IWithCallbackPlayableState<T> : IPlayableState<T>
    {
        Action EntryAction { get; set; }
        Action ExitAction { get; set; }
        Action UpdateAction { get; set; }
    }

}