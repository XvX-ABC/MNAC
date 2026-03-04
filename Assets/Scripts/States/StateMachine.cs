namespace MNAC.States
{
    public class StateMachine : StateMachine<object>
    {
        public StateMachine(string name, bool enabled = true) : base(name, enabled)
        {
        }
    }
    public class StateMachine<T> : StateMachineBase<StateBase<T>, T>
    {
        public StateMachine(string name, bool enabled = true) : base(name, enabled)
        {
        }
    }

}
