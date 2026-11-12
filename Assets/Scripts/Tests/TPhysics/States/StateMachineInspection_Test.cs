using MNAC.States;
using NUnit.Framework;

namespace MNAC.Tests.TPhysics
{
    /// <summary>StateMachine 只读快照 / 转移条件缓存回归。</summary>
    public class StateMachineInspection_Test
    {
        class ProbeState : PlayableStateBase<string>
        {
            public ProbeState(string name) : base(name, 1f)
            {
            }
        }

        [Test]
        public void CoreMachine_ExposesStatesAndLastTriggerResult()
        {
            var machine = new StateMachine<string>("core");
            var a = new EmptyState<string>("A");
            var b = new EmptyState<string>("B");
            machine.AddState(a);
            machine.AddState(b);
            machine.SetInitialState(a);

            var flag = false;
            machine.AddTransition(a, b, () => flag);

            var inspection = (IStateMachineInspection)machine;
            Assert.AreEqual("core", inspection.Name);
            Assert.AreEqual(2, inspection.StateSnapshots.Count);
            Assert.IsNull(inspection.CurrentStateName);

            flag = true;
            machine.Update(0.016f);

            Assert.AreEqual("B", inspection.CurrentStateName);
            Assert.AreEqual(1, inspection.TransitionSnapshots.Count);

            var transition = inspection.TransitionSnapshots[0];
            Assert.AreEqual("A", transition.SourceName);
            Assert.AreEqual("B", transition.DestinationName);
            Assert.IsTrue(transition.HasTriggerResult);
            Assert.IsTrue(transition.LastTriggerResult);
        }

        [Test]
        public void PlayableMachine_ExposesActiveTransitionAndProgress()
        {
            var machine = new PlayableStateMachine<string>("playable");
            var a = new ProbeState("A");
            var b = new ProbeState("B");
            machine.AddState(a);
            machine.AddState(b);
            machine.SetInitialState(a);
            machine.AddTransition(a, b, 0.2f, () => true);

            var inspection = (IStateMachineInspection)machine;
            Assert.IsFalse(inspection.IsTransitioning);

            machine.Update(0.1f);

            Assert.IsTrue(inspection.IsTransitioning);
            Assert.AreEqual("A", inspection.ActiveTransitionSource);
            Assert.AreEqual("B", inspection.ActiveTransitionDestination);
            Assert.Greater(inspection.ActiveTransitionProgress, 0.4f);
            Assert.IsTrue(inspection.TransitionSnapshots[0].IsActive);

            machine.Update(0.3f);

            Assert.IsFalse(inspection.IsTransitioning);
            Assert.AreEqual("B", inspection.CurrentStateName);
        }

        [Test]
        public void Transition_ClearsCachedResultWhenTriggersChange()
        {
            var machine = new StateMachine<string>("cache");
            var a = new EmptyState<string>("A");
            var b = new EmptyState<string>("B");
            machine.AddState(a);
            machine.AddState(b);
            var transition = machine.AddTransition(a, b, () => true);
            Assert.IsTrue(transition.IsTriggered);
            Assert.IsTrue(transition.HasTriggerResult);
            transition.AddTrigger(() => false);
            Assert.IsFalse(transition.HasTriggerResult);
        }
    }
}
