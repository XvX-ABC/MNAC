using UnityEngine;

namespace MNAC.States
{
    /// <summary>
    /// 转移矩阵 Inspector 的示例宿主：用 <see cref="PlayableStateMachineBlueprint{T}"/>
    /// 描述状态图，运行时 Build 出一台 Playable 状态机。
    /// 挂到任意 GameObject 上即可在 Inspector 中看到矩阵和过渡进度。
    /// </summary>
    [AddComponentMenu("MNAC/States/State Machine Debug Example")]
    public sealed class StateMachineDebugExample : StateMachineHost
    {
        sealed class ExampleState : PlayableStateBase<object>
        {
            public ExampleState(string name, float duration) : base(name, duration)
            {
            }
        }

        PlayableStateMachine<object> machine;

        public override IStateMachineInspection Inspection
        {
            get
            {
                EnsureMachine();
                return machine;
            }
        }

        protected override void Tick(float deltaTime)
        {
            EnsureMachine();
            machine.Update(deltaTime);
        }

        void Awake()
        {
            EnsureMachine();
        }

        void EnsureMachine()
        {
            if (machine != null)
                return;

            var blueprint = new PlayableStateMachineBlueprint<object>("Example")
                .State("Idle", () => new ExampleState("Idle", 1f))
                .State("Run", () => new ExampleState("Run", 1.2f))
                .State("Jump", () => new ExampleState("Jump", 0.15f))
                .State("Fall", () => new ExampleState("Fall", 0.2f))
                .State("Land", () => new ExampleState("Land", 0.3f))
                .Entry("Idle")
                .PlayableTransition("Idle", "Run", 0.15f, _ => Time.time % 6f > 1f)
                .PlayableTransition("Run", "Jump", 0.10f, _ => Time.time % 6f > 2.5f)
                .PlayableTransition("Jump", "Fall", 0.10f, _ => Time.time % 6f > 2.7f)
                .PlayableTransition("Fall", "Land", 0.10f, _ => Time.time % 6f > 2.9f)
                .PlayableTransition("Land", "Idle", 0.20f, _ => Time.time % 6f > 3.5f)
                .PlayableTransition("Run", "Idle", 0.15f, _ => Time.time % 6f > 4.5f);

            machine = blueprint.Build(null);
        }
    }
}
