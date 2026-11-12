using UnityEngine;

namespace MNAC.States
{
    /// <summary>
    /// 转移矩阵 Inspector 的示例宿主：自动构建一个小型 Playable 状态机，
    /// 挂到任意 GameObject 上即可在 Inspector 中看到矩阵和过渡进度。
    /// 正式项目请按同样的方式实现自己的 StateMachineHost。
    /// </summary>
    [AddComponentMenu("MNAC/States/State Machine Debug Example")]
    public sealed class StateMachineDebugExample : StateMachineHost
    {
        sealed class ExampleState : PlayableStateBase
        {
            public ExampleState(string name, float duration) : base(name, duration)
            {
            }
        }

        PlayableStateMachine machine;

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

            machine = new PlayableStateMachine("Example");
            var idle = new ExampleState("Idle", 1f);
            var run = new ExampleState("Run", 1.2f);
            var jump = new ExampleState("Jump", 0.15f);
            var fall = new ExampleState("Fall", 0.2f);
            var land = new ExampleState("Land", 0.3f);

            machine.AddState(idle);
            machine.AddState(run);
            machine.AddState(jump);
            machine.AddState(fall);
            machine.AddState(land);
            machine.SetInitialState(idle);

            machine.AddTransition(idle, run, 0.15f, () => Time.time % 6f > 1f);
            machine.AddTransition(run, jump, 0.10f, () => Time.time % 6f > 2.5f);
            machine.AddTransition(jump, fall, 0.10f, () => Time.time % 6f > 2.7f);
            machine.AddTransition(fall, land, 0.10f, () => Time.time % 6f > 2.9f);
            machine.AddTransition(land, idle, 0.20f, () => Time.time % 6f > 3.5f);
            machine.AddTransition(run, idle, 0.15f, () => Time.time % 6f > 4.5f);
        }
    }
}
