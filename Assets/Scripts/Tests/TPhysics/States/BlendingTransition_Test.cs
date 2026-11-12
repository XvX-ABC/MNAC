using MNAC.States;
using NUnit.Framework;

namespace MNAC.Tests.TPhysics
{
    /// <summary>Blending / SubStateMachine 的长度恢复与回调平衡回归。</summary>
    public class BlendingTransition_Test
    {
        class CallbackState : WithCallbackPlayableState<string>
        {
            public CallbackState(string name, float duration) : base(name, duration)
            {
            }
        }

        [Test]
        public void Options_FixedExitTime_SetterEnables()
        {
            var options = new BlendingTransitionOptions { FixedExitTime = 0.5f };
            Assert.IsTrue(options.EnableFixedExit);
            Assert.AreEqual(0.5f, options.FixedExitTime, 1e-5f);

            options.FixedExitTime = BlendingTransition<object>.FIXED_EXIT_TIME_INVALID_VALUE;
            Assert.IsFalse(options.EnableFixedExit);
            Assert.AreEqual(BlendingTransition<object>.FIXED_EXIT_TIME_INVALID_VALUE, options.FixedExitTime, 1e-5f);
        }

        [Test]
        public void BeginTwice_RestoresOriginalLength_AndReapplies()
        {
            var source = new CallbackState("source", 1f);
            var destination = new CallbackState("destination", 1f);
            var transition = new BlendingTransition<string>(
                source, destination, duration: 0.2f, offset: 0f,
                fixedExitTime: BlendingTransition<string>.FIXED_EXIT_TIME_INVALID_VALUE,
                interruptionMode: InterruptionMode.None);

            transition.Timeline.Restart();
            transition.Timeline.OnUpdate(0f);
            Assert.AreEqual(0.8f, destination.Timeline.Length, 1e-5f);

            // 模拟中断/重放：Begin 必须先把上一次的 0.8 恢复成 1，再缩短成 0.8
            transition.Timeline.Restart();
            transition.Timeline.OnUpdate(0f);
            Assert.AreEqual(0.8f, destination.Timeline.Length, 1e-5f);

            destination.OnExit();
            Assert.AreEqual(1f, destination.Timeline.Length, 1e-5f, "退出后应恢复原始长度，而不是上一次缩短后的长度");
        }

        [Test]
        public void OnInterrupted_RestoresDestinationLength()
        {
            var source = new CallbackState("source", 1f);
            var destination = new CallbackState("destination", 1f);
            var transition = new BlendingTransition<string>(source, destination, duration: 0.2f);

            transition.Timeline.Restart();
            transition.Timeline.OnUpdate(0f);
            Assert.AreEqual(0.8f, destination.Timeline.Length, 1e-5f);

            ((IInterruptiblePlayableTransition<string>)transition).OnInterrupted();
            Assert.AreEqual(1f, destination.Timeline.Length, 1e-5f, "中断时也必须恢复目标长度");
        }

        [Test]
        public void SubStateMachineTransition_AppliesBlendingToNewTarget_AndRestoresOld()
        {
            var subMachine = new WithCallbackPlayableStateMachine<string>("sub");
            var a = new CallbackState("a", 1f);
            var b = new CallbackState("b", 1f);
            subMachine.AddState(a);
            subMachine.AddState(b);
            subMachine.SetInitialState(a);
            subMachine.OnEnter();

            var source = new CallbackState("source", 1f);
            var transition = new SubStateMachineTransition<string>(
                source, subMachine, b, duration: 0.2f, offset: 0.1f,
                fixedExitTime: SubStateMachineTransition<string>.FIXED_EXIT_TIME_INVALID_VALUE,
                interruptionMode: InterruptionMode.None);

            transition.Timeline.Restart();
            transition.Timeline.OnUpdate(0f);

            Assert.AreSame(b, subMachine.CurrentState);
            Assert.AreEqual(1f, a.Timeline.Length, 1e-5f, "旧状态不应残留缩短");
            Assert.AreEqual(0.7f, b.Timeline.Length, 1e-5f, "新目标应应用 offset + duration");
            Assert.AreEqual(0.7f, subMachine.Timeline.Length, 1e-5f);

            b.OnExit();
            Assert.AreEqual(1f, b.Timeline.Length, 1e-5f, "目标退出后应恢复原始长度");
        }

        [Test]
        public void SubStateMachineState_EntryAndExitCallbacksAreBalanced()
        {
            var child = new WithCallbackPlayableStateMachine<string>("child");
            var entries = 0;
            var exits = 0;
            child.EntryAction = () => entries++;
            child.ExitAction = () => exits++;

            var a = new CallbackState("a", 1f);
            var b = new CallbackState("b", 1f);
            child.AddState(a);
            child.AddState(b);
            child.SetInitialState(a);

            var wrapper = new SubStateMachineState<string>(child, b, "wrap");
            wrapper.OnEnter();
            Assert.AreEqual(1, entries, "包装状态进入时应触发子机 EntryAction");
            Assert.AreSame(b, child.CurrentState);

            wrapper.OnExit();
            Assert.AreEqual(1, exits, "包装状态退出时应触发子机 ExitAction");
        }

        [Test]
        public void SubStateMachineState_RejectsUnregisteredTarget()
        {
            var child = new WithCallbackPlayableStateMachine<string>("child");
            var a = new CallbackState("a", 1f);
            var stranger = new CallbackState("stranger", 1f);
            child.AddState(a);
            child.SetInitialState(a);

            Assert.Throws<StateNotRegisteredException>(() => new SubStateMachineState<string>(child, stranger, "wrap"));
        }

        [Test]
        public void SubStateMachineTransition_RejectsUnregisteredTarget()
        {
            var child = new WithCallbackPlayableStateMachine<string>("child");
            var a = new CallbackState("a", 1f);
            var stranger = new CallbackState("stranger", 1f);
            child.AddState(a);
            child.SetInitialState(a);
            var source = new CallbackState("source", 1f);

            Assert.Throws<StateNotRegisteredException>(() =>
                new SubStateMachineTransition<string>(source, child, stranger));
        }

        [Test]
        public void PlayableStateMachineState_TimelineDelegatesToChild()
        {
            var child = new WithCallbackPlayableStateMachine<string>("child");
            var a = new CallbackState("a", 0.5f);
            child.AddState(a);
            child.SetInitialState(a);
            child.OnEnter();

            var wrapper = new PlayableStateMachineState<string>(child, "wrap", 1f);
            wrapper.OnEnter();

            Assert.AreEqual(0.5f, wrapper.Timeline.Length, 1e-5f, "包装层 Timeline 应指向子机当前状态");
        }
    }
}
