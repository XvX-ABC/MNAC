using System;

namespace MNAC.StatesNew
{
    /// <summary>非泛型薄壳，等价于 <see cref="WithCallbackPlayableStateMachine{T}"/> with T=object。</summary>
    public class WithCallbackPlayableStateMachine : WithCallbackPlayableStateMachine<object>
    {
        public WithCallbackPlayableStateMachine(string name, bool enabled = true) : base(name, enabled)
        {
        }
    }

    /// <summary>
    /// 带回调的可播放状态机：在 <see cref="PlayableStateMachine{T}"/> 基础上支持
    /// EntryAction/UpdateAction/ExitAction 三个委托，外部无需子类化即可挂逻辑。
    /// 同时实现 <see cref="IWithCallbackPlayableState{T}"/>，可被外层机器当状态用。
    /// </summary>
    public class WithCallbackPlayableStateMachine<T> : PlayableStateMachine<T>, IWithCallbackPlayableState<T>
    {
        Action entryAction;
        Action updateAction;
        Action exitAction;

        public WithCallbackPlayableStateMachine(string name, bool enabled = true) : base(name, enabled)
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
