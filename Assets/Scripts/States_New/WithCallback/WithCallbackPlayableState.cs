using System;

namespace MNAC.StatesNew
{
    /// <summary>非泛型薄壳，等价于 <see cref="WithCallbackPlayableState{T}"/> with T=object。</summary>
    public abstract class WithCallbackPlayableState : WithCallbackPlayableState<object>
    {
        public WithCallbackPlayableState(string name, float duration = 0, bool enabled = true) : base(name, duration, enabled)
        {
        }
    }

    /// <summary>
    /// 带回调的可播放状态：OnEnter/OnUpdate/OnExit 分别先/后调用 EntryAction/UpdateAction/ExitAction，
    /// 并保留基类的 Timeline 自动推进。
    /// </summary>
    public abstract class WithCallbackPlayableState<T> : PlayableStateBase<T>, IWithCallbackPlayableState<T>
    {
        Action entryAction;
        Action updateAction;
        Action exitAction;

        public WithCallbackPlayableState(string name, float duration = 0, bool enabled = true) : base(name, duration, enabled)
        {
        }

        public Action EntryAction { get => entryAction; set => entryAction = value; }
        public Action UpdateAction { get => updateAction; set => updateAction = value; }
        public Action ExitAction { get => exitAction; set => exitAction = value; }

        public override void OnEnter()
        {
            entryAction?.Invoke();
            base.OnEnter();
        }

        public override void OnUpdate(float deltaTime)
        {
            updateAction?.Invoke();
            base.OnUpdate(deltaTime);
        }

        public override void OnExit()
        {
            base.OnExit();
            exitAction?.Invoke();
        }
    }
}
