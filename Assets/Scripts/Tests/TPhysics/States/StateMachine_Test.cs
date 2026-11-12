using System;
using MNAC.States;
using NUnit.Framework;

namespace MNAC.Tests.TPhysics
{
    /// <summary>Core StateMachine 生命周期/异常包装回归。</summary>
    public class StateMachine_Test
    {
        class TrackingState : StateBase<string>
        {
            public int Enters;
            public int Updates;
            public int Exits;

            public TrackingState(string name) : base(name)
            {
            }

            public override void OnEnter() => Enters++;
            public override void OnUpdate(float deltaTime) => Updates++;
            public override void OnExit() => Exits++;
        }

        class ThrowingUpdateState : TrackingState
        {
            public ThrowingUpdateState(string name) : base(name)
            {
            }

            public override void OnUpdate(float deltaTime) => throw new InvalidOperationException("update-boom");
        }

        class ThrowingEnterState : TrackingState
        {
            public ThrowingEnterState(string name) : base(name)
            {
            }

            public override void OnEnter() => throw new InvalidOperationException("enter-boom");
        }

        [Test]
        public void Update_OnUpdateThrows_WrapsStateUpdateException()
        {
            var machine = new StateMachine<string>("m");
            var state = new ThrowingUpdateState("bad");
            machine.AddState(state);
            machine.SetInitialState(state);

            var exception = Assert.Throws<StateUpdateException>(() => machine.Update(0.1f));
            StringAssert.Contains("bad", exception.Message);
        }

        [Test]
        public void Update_TriggerThrows_WrapsStateTransitionException()
        {
            var machine = new StateMachine<string>("m");
            var from = new TrackingState("from");
            var to = new TrackingState("to");
            machine.AddState(from);
            machine.AddState(to);
            machine.SetInitialState(from);
            machine.AddTransition(from, to, () => throw new InvalidOperationException("trigger-boom"));

            Assert.Throws<StateTransitionException>(() => machine.Update(0.1f));
        }

        [Test]
        public void Update_OnTriggeredThrows_WrapsStateTransitionException()
        {
            var machine = new StateMachine<string>("m");
            var from = new TrackingState("from");
            var to = new TrackingState("to");
            machine.AddState(from);
            machine.AddState(to);
            machine.SetInitialState(from);
            machine.AddTransition(from, to, null, _ => throw new InvalidOperationException("on-triggered-boom"));

            Assert.Throws<StateTransitionException>(() => machine.Update(0.1f));
        }

        [Test]
        public void SwitchTo_OnEnterThrows_ClearsCurrent_AndPreviousAlreadyExited()
        {
            var machine = new StateMachine<string>("m");
            var good = new TrackingState("good");
            var fail = new ThrowingEnterState("fail");
            machine.AddState(good);
            machine.AddState(fail);
            machine.SetInitialState(good);
            machine.Update(0f);

            Assert.Throws<StateEnterException>(() => machine.ChangeStateTo(fail));
            Assert.IsNull(machine.CurrentState, "Enter 失败后 current 不应继续指向已退出的旧状态");
            Assert.AreEqual(1, good.Exits);
        }

        [Test]
        public void RemoveState_Current_CallsOnExit_AndClearsCurrent()
        {
            var machine = new StateMachine<string>("m");
            var state = new TrackingState("s");
            machine.AddState(state);
            machine.SetInitialState(state);
            machine.Update(0f);

            Assert.IsTrue(machine.RemoveState(state));
            Assert.AreEqual(1, state.Exits, "移除当前状态前必须调用 OnExit");
            Assert.IsNull(machine.CurrentState);
            Assert.IsFalse(machine.Contains(state));
        }
    }
}
