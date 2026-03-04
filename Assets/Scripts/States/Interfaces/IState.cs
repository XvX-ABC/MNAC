using System;

namespace MNAC.States
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
}
