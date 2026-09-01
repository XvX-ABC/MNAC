namespace MNAC.States
{
    public class AnimationStateMachine<T> : StateMachine<T>
    {
        public AnimationStateMachine(string name) : base(name)
        {
        }

        public new AnimationStateBase<T> CurrentState { get => (AnimationStateBase<T>)currentState; }
        public void OnAnimatorIK(int layerIndex)
        {
            if (CurrentState == null)
                return;
            CurrentState.OnAnimationIK(layerIndex);
        }
        public void AddState(AnimationStateBase<T> state) => base.AddState(state);
    }
}
