using MNAC.States;
using NUnit.Framework;

namespace MNAC.Tests.TPhysics
{
    /// <summary>方案 A：Blueprint / keyed 状态 / 上下文条件的回归测试。</summary>
    public class StateMachineBlueprint_Test
    {
        sealed class ProbeState : StateBase<string>
        {
            public ProbeState(string name) : base(name)
            {
            }

            public override void OnEnter()
            {
            }

            public override void OnUpdate(float deltaTime)
            {
            }

            public override void OnExit()
            {
            }
        }

        sealed class ProbePlayableState : PlayableStateBase<string>
        {
            public ProbePlayableState(string name) : base(name, 0f)
            {
            }
        }

        [Test]
        public void Build_CreatesIndependentStateInstances()
        {
            var creates = 0;
            var blueprint = new StateMachineBlueprint<string>("independent")
                .State("a", () =>
                {
                    creates++;
                    return new ProbeState("A");
                })
                .State("b", () => new ProbeState("B"))
                .Entry("a")
                .Transition("a", "b", _ => true);

            var first = blueprint.Build("first");
            var second = blueprint.Build("second");

            Assert.AreEqual(2, creates, "每次 Build 都必须重新执行状态工厂");
            Assert.AreNotSame(first.GetState("a"), second.GetState("a"));
            Assert.AreEqual("first", first.GetState("a").Context);
            Assert.AreEqual("second", second.GetState("a").Context);

            first.Update(0f);
            Assert.AreEqual("B", first.CurrentState.Name);
            Assert.IsNull(second.CurrentState, "第一台机器推进不应影响第二台");
        }

        [Test]
        public void Build_ConditionReceivesBuildContext()
        {
            var blueprint = new StateMachineBlueprint<string>("context")
                .State("a", () => new ProbeState("A"))
                .State("b", () => new ProbeState("B"))
                .Entry("a")
                .Transition("a", "b", context => context == "go");

            var stay = blueprint.Build("stay");
            stay.Update(0f);
            Assert.AreEqual("A", stay.CurrentState.Name);

            var go = blueprint.Build("go");
            go.Update(0f);
            Assert.AreEqual("B", go.CurrentState.Name);
        }

        [Test]
        public void Build_InvokesOnTriggeredWithContext()
        {
            string seen = null;
            var blueprint = new StateMachineBlueprint<string>("triggered")
                .State("a", () => new ProbeState("A"))
                .State("b", () => new ProbeState("B"))
                .Entry("a")
                .Transition("a", "b", _ => true, context => seen = context);

            blueprint.Build("payload").Update(0f);

            Assert.AreEqual("payload", seen);
        }

        [Test]
        public void Build_WithoutEntry_UsesFirstRegisteredState()
        {
            var blueprint = new StateMachineBlueprint<string>("default-entry")
                .State("first", () => new ProbeState("First"))
                .State("second", () => new ProbeState("Second"));

            var machine = blueprint.Build("ctx");
            machine.Update(0f);

            Assert.AreEqual("First", machine.CurrentState.Name);
        }

        [Test]
        public void State_DuplicateKey_Throws()
        {
            var blueprint = new StateMachineBlueprint<string>("duplicate")
                .State("a", () => new ProbeState("A"));

            Assert.Throws<DuplicateStateKeyException>(
                () => blueprint.State("a", () => new ProbeState("Another")));
        }

        [Test]
        public void Build_TransitionWithMissingKey_Throws()
        {
            var blueprint = new StateMachineBlueprint<string>("missing")
                .State("a", () => new ProbeState("A"))
                .Transition("a", "missing");

            Assert.Throws<StateKeyNotFoundException>(() => blueprint.Build("ctx"));
        }

        [Test]
        public void Build_EntryWithMissingKey_Throws()
        {
            var blueprint = new StateMachineBlueprint<string>("missing-entry")
                .State("a", () => new ProbeState("A"))
                .Entry("nope");

            Assert.Throws<StateKeyNotFoundException>(() => blueprint.Build("ctx"));
        }

        [Test]
        public void StateMachine_KeyedApi_RegistersFindsAndRemoves()
        {
            var machine = new StateMachine<string>("keys");
            var state = new ProbeState("A");

            machine.AddState("a", state);

            Assert.AreSame(state, machine.FindState("a"));
            Assert.AreSame(state, machine.GetState("a"));
            Assert.IsNull(machine.FindState("missing"));
            Assert.Throws<StateKeyNotFoundException>(() => machine.GetState("missing"));
            Assert.Throws<DuplicateStateKeyException>(
                () => machine.AddState("a", new ProbeState("B")));

            Assert.IsTrue(machine.RemoveState(state));
            Assert.IsNull(machine.FindState("a"));
        }

        [Test]
        public void Transition_LegacyTriggerAndContextCondition_AreAnded()
        {
            var machine = new StateMachine<string>("conditions");
            var a = new ProbeState("A");
            var b = new ProbeState("B");
            machine.AddState(a);
            machine.AddState(b);

            var transition = machine.AddTransition(a, b, () => true);
            transition.AddCondition(new DelegateTransitionCondition<string>(context => context == "ok"));

            Assert.AreEqual(2, transition.TriggerCount);
            Assert.IsFalse(transition.Evaluate("no"));
            Assert.IsTrue(transition.Evaluate("ok"));
        }

        [Test]
        public void PlayableBlueprint_CreatesIndependentTimelines()
        {
            var blueprint = new PlayableStateMachineBlueprint<string>("playable")
                .State("a", () => new ProbePlayableState("A"))
                .State("b", () => new ProbePlayableState("B"))
                .Entry("a")
                .PlayableTransition("a", "b", 0.2f, _ => true);

            var first = blueprint.Build("ctx");
            var second = blueprint.Build("ctx");

            var firstTransition = (PlayableTransition<string>)first.Transitions[0];
            var secondTransition = (PlayableTransition<string>)second.Transitions[0];

            Assert.AreNotSame(firstTransition, secondTransition);
            Assert.AreNotSame(firstTransition.Timeline, secondTransition.Timeline);
            Assert.AreEqual(0.2f, firstTransition.Timeline.Length, 0.0001f);

            first.Update(0.1f);
            Assert.AreEqual("A", ((IStateMachineInspection)first).ActiveTransitionSource);
            Assert.IsFalse(((IStateMachineInspection)second).IsTransitioning);

            second.Update(0.1f);
            Assert.AreEqual("A", ((IStateMachineInspection)second).ActiveTransitionSource);
        }

        [Test]
        public void PlayableBlueprint_NonPlayableState_Throws()
        {
            var blueprint = new PlayableStateMachineBlueprint<string>("invalid")
                .State("a", () => new ProbeState("A"))
                .State("b", () => new ProbeState("B"))
                .Transition("a", "b");

            Assert.Throws<System.InvalidCastException>(() => blueprint.Build("ctx"));
        }
    }
}
