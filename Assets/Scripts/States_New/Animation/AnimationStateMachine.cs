namespace MNAC.StatesNew
{
    /// <summary>动画状态机：转发 OnAnimatorIK 给当前动画状态。</summary>
    public class AnimationStateMachine<T> : StateMachine<T>
    {
        public AnimationStateMachine(string name) : base(name)
        {
        }

        public new AnimationStateBase<T> CurrentState => current as AnimationStateBase<T>;

        public void OnAnimatorIK(int layerIndex)
        {
            if (CurrentState == null)
                return;
            CurrentState.OnAnimationIK(layerIndex);
        }
    }
}
