using Tests.Locomotion.Animation;

namespace Tests.States
{
    public class AnimationStateMachine<T> : StateMachine<T>
    {
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
