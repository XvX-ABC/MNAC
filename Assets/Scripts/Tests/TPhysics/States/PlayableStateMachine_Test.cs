using MNAC.States;
using NUnit.Framework;

namespace MNAC.Tests.TPhysics
{
    /// <summary>
    /// Timeline 与 PlayableStateMachine 的集成回归：
    /// Timeline 的 End 比 NormalizedTime==1 晚一拍，过渡状态若只按 NormalizedTime 判定完成，
    /// 会在 EndAction 前切走状态，导致 End 回调丢失、Timeline 永远停在 IsRunning。
    /// </summary>
    public class PlayableStateMachine_Test
    {
        class ProbeState : PlayableStateBase<string>
        {
            public int ToNextEndCount;
            public int FromPreviousEndCount;

            public ProbeState(string name) : base(name, 1f)
            {
            }

            public override void ToNextStateTransitionEnd(IReadonlyPlayableTransition<string> transition)
            {
                ToNextEndCount++;
            }

            public override void FromPreviousStateTransitionEnd(IReadonlyPlayableTransition<string> transition)
            {
                FromPreviousEndCount++;
            }
        }

        [Test]
        public void Transition_WithDuration_FiresEndCallbacks_AndTimelineCompletes()
        {
            var machine = new PlayableStateMachine<string>("test");
            var from = new ProbeState("from");
            var to = new ProbeState("to");
            machine.AddState(from);
            machine.AddState(to);
            machine.SetInitialState(from);

            var transition = machine.AddTransition(from, to, 0.2f, () => true);
            var timelineEnds = 0;
            transition.Timeline.EndAction += _ => timelineEnds++;

            for (var i = 0; i < 4; i++)
                machine.Update(0.1f);

            Assert.AreSame(to, machine.CurrentState, "过渡播完应进入目标状态");
            Assert.AreEqual(1, timelineEnds, "过渡 Timeline 的 EndAction 应触发一次");
            Assert.AreEqual(1, from.ToNextEndCount, "源状态的 ToNextStateTransitionEnd 应触发");
            Assert.AreEqual(1, to.FromPreviousEndCount, "目标状态的 FromPreviousStateTransitionEnd 应触发");
            Assert.IsFalse(transition.Timeline.IsRunning, "过渡结束后 Timeline 不应卡在运行中");
        }

        [Test]
        public void Transition_Interrupted_EndsOldTimeline_AndFiresEndCallbacks()
        {
            var machine = new PlayableStateMachine<string>("test");
            var a = new ProbeState("a");
            var b = new ProbeState("b");
            var c = new ProbeState("c");
            machine.AddState(a);
            machine.AddState(b);
            machine.AddState(c);
            machine.SetInitialState(a);

            var ab = machine.AddTransition(a, b, 1f, () => true);
            machine.AddTransition(b, c, 0.1f, () => true);

            machine.Update(0.1f); // 启动 a -> b
            machine.Update(0.1f); // b -> c 立即打断

            Assert.AreEqual("b -> c", machine.CurrentState.Name);
            Assert.IsFalse(ab.Timeline.IsRunning, "被打断的旧 Timeline 必须结束，不能卡在 running");
            Assert.AreEqual(1, a.ToNextEndCount, "旧过渡源的 End 回调应触发");
            Assert.AreEqual(1, b.FromPreviousEndCount, "旧过渡目标的 End 回调应触发");

            machine.Update(0.1f);
            Assert.AreSame(c, machine.CurrentState);
            Assert.AreEqual(1, b.ToNextEndCount);
            Assert.AreEqual(1, c.FromPreviousEndCount);
        }

        class PlainTransitionMachine : PlayableStateMachine<string>
        {
            public PlainTransitionMachine(string name) : base(name)
            {
            }

            public void AddPlain(IState<string> source, IState<string> destination)
            {
                RegisterTransition(new Transition<string>(source, destination, () => true));
            }
        }

        [Test]
        public void ZeroDurationTransition_CompletesInSameUpdate()
        {
            var machine = new PlayableStateMachine<string>("test");
            var a = new ProbeState("a");
            var b = new ProbeState("b");
            machine.AddState(a);
            machine.AddState(b);
            machine.SetInitialState(a);
            machine.AddTransition(a, b, 0f, () => true);

            machine.Update(0.1f);

            Assert.AreSame(b, machine.CurrentState, "零时长过渡不应额外占用一帧");
        }

        [Test]
        public void ExternalChange_DuringTransition_EndsOldTimeline()
        {
            var machine = new PlayableStateMachine<string>("test");
            var a = new ProbeState("a");
            var b = new ProbeState("b");
            var c = new ProbeState("c");
            machine.AddState(a);
            machine.AddState(b);
            machine.AddState(c);
            machine.SetInitialState(a);
            var ab = machine.AddTransition(a, b, 1f, () => true);

            machine.Update(0.1f);
            machine.ChangeStateTo(c);

            Assert.AreSame(c, machine.CurrentState);
            Assert.IsFalse(ab.Timeline.IsRunning, "外部切走过渡状态时也要结束旧 Timeline");
            Assert.AreEqual(1, a.ToNextEndCount);
            Assert.AreEqual(1, b.FromPreviousEndCount);
        }

        [Test]
        public void DirectInterruption_EndsOldTimeline_AndEntersDestination()
        {
            var machine = new PlainTransitionMachine("test");
            var a = new ProbeState("a");
            var b = new ProbeState("b");
            var c = new ProbeState("c");
            machine.AddState(a);
            machine.AddState(b);
            machine.AddState(c);
            machine.SetInitialState(a);
            var ab = machine.AddTransition(a, b, 1f, () => true);
            machine.AddPlain(b, c);

            machine.Update(0.1f); // 启动 a -> b
            machine.Update(0.1f); // b -> c 直切打断

            Assert.AreSame(c, machine.CurrentState);
            Assert.IsFalse(ab.Timeline.IsRunning);
            Assert.AreEqual(1, a.ToNextEndCount);
            Assert.AreEqual(1, b.FromPreviousEndCount);
        }

        [Test]
        public void Completion_DoesNotAdvanceDestinationByTransitionDelta()
        {
            var machine = new PlayableStateMachine<string>("test");
            var a = new ProbeState("a");
            var b = new ProbeState("b");
            machine.AddState(a);
            machine.AddState(b);
            machine.SetInitialState(a);
            machine.AddTransition(a, b, 0.2f, () => true);

            machine.Update(0.1f);
            machine.Update(0.1f);

            Assert.AreSame(b, machine.CurrentState);
            Assert.AreEqual(0f, b.Timeline.Time, 1e-5f, "完成帧不应把过渡的 delta 再喂给目标状态");
        }
    }
}
