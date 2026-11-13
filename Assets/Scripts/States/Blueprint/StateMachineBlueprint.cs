namespace MNAC.States
{
    /// <summary>非泛型薄壳，等价于 <see cref="StateMachineBlueprint{T}"/> with T=object。</summary>
    public class StateMachineBlueprint : StateMachineBlueprint<object>
    {
        public StateMachineBlueprint(string name) : base(name)
        {
        }
    }

    /// <summary>纯逻辑状态机的图描述，Build 产出 <see cref="StateMachine{T}"/>。</summary>
    public class StateMachineBlueprint<T>
        : StateMachineBlueprintBase<T, StateMachine<T>, StateMachineBlueprint<T>>
    {
        public StateMachineBlueprint(string name) : base(name)
        {
        }

        protected override StateMachine<T> CreateMachine() => new StateMachine<T>(Name);
    }
}
