using Assets.Scripts.Utilities.Timeline;
using System;

namespace Tests.States
{
    public interface IState<T>
    {
        public string Name { get; }
        public T Context { get; set; }
        public bool Enabled { get; set; }
        public Guid ID { get; }
        public ITransition<T>[] Transitions { get; }
        public void AddTransition(ITransition<T> transition);
        public void RemoveTransition(IState<T> destinationState);
        public ITransition<T> FindTransition(IState<T> destinationState);
        public void OnEnter();
        public void OnExit();
        public void OnUpdate();
    }
    public interface IState : IState<object>
    {
    }

    public interface IPlayableState<T> : IState<T>
    {
        public ITimeline Timeline { get; }
        public bool ExitWhenEnd { get; set; }
        public new IPlayableTransition<T>[] Transitions { get; }
        public void TransitionBeginWhichOfPreviousState(IReadonlyPlayableTransition<T> currentTransition);
        public void TransitionRunningWhichOfPreviousState(IReadonlyPlayableTransition<T> currentTransition);
        public void TransitionEndWhichOfPreviousState(IReadonlyPlayableTransition<T> currentTransition);
        public void TransitionBeginWhichToNextState(IReadonlyPlayableTransition<T> currentTransition);
        public void TransitionRunningWhichToNextState(IReadonlyPlayableTransition<T> currentTransition);
        public void TransitionEndWhichToNextState(IReadonlyPlayableTransition<T> currentTransition);
    }
    public interface IAnimationState<T> : IPlayableState<T>
    {
        public byte State { get; }
        public void SetTime(float time);
        public void Play();
        public void Pause();
        public void Reset();
    }
}
