using Tests.States;
namespace Tests.Characters.Humanoid.Animations
{
    internal class NormalState : SubStatemachineState<object>
    {
        public NormalState(WithCallbackPlayableStatemachine<object> statemachine, IWithCallbackPlayableState<object> targetState, string name, bool enabled = true) : base(statemachine, targetState, name, enabled)
        {
        }
    }
}
