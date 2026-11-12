using System.Text.RegularExpressions;
using MNAC.Utilities.Timeline;
using MNAC.Utilities.Timeline.Events;
using MNAC.Utilities.Timeline.Events.Point;
using MNAC.Utilities.Timeline.Events.Range;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace MNAC.Tests.TPhysics
{
    /// Timeline 核心行为测试（纯逻辑，EditMode，显式传 deltaTime）。
    public class Timeline_Test
    {
        [Test]
        public void Update_AdvancesTime_AndCompletesAtLength()
        {
            var timeline = new Timeline(1f);
            var starts = 0;
            var ends = 0;
            timeline.StartAction += _ => starts++;
            timeline.EndAction += _ => ends++;

            timeline.Restart();
            timeline.OnUpdate(0.25f);

            Assert.AreEqual(0.25f, timeline.Time, 1e-5f);
            Assert.AreEqual(0.25f, timeline.NormalizedTime, 1e-5f);
            Assert.AreEqual(1, starts);
            Assert.IsTrue(timeline.IsRunning);

            // 0.25 → 0.5 → 0.75 → 1.0，再下一拍在 time == length 处触发 End
            for (var i = 0; i < 4; i++)
                timeline.OnUpdate(0.25f);

            Assert.AreEqual(1f, timeline.Time, 1e-5f);
            Assert.AreEqual(1, ends);
            Assert.AreEqual(1, starts);
            Assert.IsFalse(timeline.IsRunning);
        }

        [Test]
        public void StartAction_FiresOncePerRun()
        {
            var timeline = new Timeline(1f);
            var starts = 0;
            timeline.StartAction += _ => starts++;

            timeline.Restart();
            timeline.OnUpdate(0.1f);
            timeline.OnUpdate(0.1f);
            Assert.AreEqual(1, starts, "同一次运行内 Start 只触发一次");

            timeline.Reset();
            timeline.Restart();
            timeline.OnUpdate(0.1f);
            Assert.AreEqual(2, starts, "Reset + Restart 后应重新触发 Start");
        }

        [Test]
        public void PointEvent_FiresOnce_AtConfiguredProportion()
        {
            var timeline = new Timeline(1f);
            var fired = 0;
            timeline.AddPointEvent(0.5f, _ => fired++);

            timeline.Restart();
            for (var i = 0; i < 10; i++)
                timeline.OnUpdate(0.1f);

            Assert.AreEqual(1, fired, "点事件命中后不应重复触发");
        }

        [Test]
        public void RangeEvent_FiresOnlyInsideRange()
        {
            var timeline = new Timeline(1f);
            var fired = 0;
            timeline.AddRangeEvent(0.3f, 0.2f, _ => fired++);

            timeline.Restart();
            for (var i = 0; i < 11; i++)
                timeline.OnUpdate(0.1f);

            // 归一化时刻 0,.1,...,.9,1.0；闭区间 [0.3, 0.5] 内为 .3、.4、.5
            Assert.AreEqual(3, fired);
        }

        [Test]
        public void LoopTimeline_WrapsAndKeepsRunning()
        {
            var timeline = new Timeline(1f, isLoop: true);
            var starts = 0;
            timeline.StartAction += _ => starts++;

            timeline.Restart();
            timeline.OnUpdate(0.6f); // 0 → 0.6
            timeline.OnUpdate(0.6f); // 0.6 → 1.2
            timeline.OnUpdate(0.6f); // 1.2 ≥ 1 → 归零并保留余量

            Assert.IsTrue(timeline.IsRunning, "循环时间线不应结束");
            Assert.AreEqual(0.2f, timeline.Time, 1e-4f);
            Assert.AreEqual(1, starts);

            timeline.OnUpdate(0.1f);
            Assert.AreEqual(2, starts, "进入下一循环后应重新触发 Start");
        }

        [Test]
        public void LoopTimeline_LargeDelta_ResyncsInOneWrap()
        {
            var timeline = new Timeline(1f, isLoop: true);
            var starts = 0;
            timeline.StartAction += _ => starts++;

            timeline.Restart();
            timeline.OnUpdate(3.5f); // 一次跨过 3 个循环
            timeline.OnUpdate(0f);   // 应一次取模回到 [0, 1)

            Assert.IsTrue(timeline.IsRunning);
            Assert.AreEqual(0.5f, timeline.Time, 1e-4f, "大 delta 应一次取模，而不是逐帧追帧");
            Assert.AreEqual(1, starts);
        }

        [Test]
        public void SetTimeAndNormalizedTime_Clamp_AndRejectWhileRunning()
        {
            var timeline = new Timeline(2f);

            Assert.IsTrue(timeline.SetTime(5f));
            Assert.AreEqual(2f, timeline.Time, 1e-5f, "SetTime 应钳制到 [0, length]");

            Assert.IsTrue(timeline.SetNormalizedTime(0.25f));
            Assert.AreEqual(0.5f, timeline.Time, 1e-5f);

            timeline.Restart();
            LogAssert.Expect(LogType.Warning, new Regex("can't set time"));
            Assert.IsFalse(timeline.SetTime(0.5f), "运行中修改时间应被拒绝");
        }

        [Test]
        public void UpdateLength_ChangesLength_BeforeRunning_AndRejectedWhileRunning()
        {
            var timeline = new Timeline(1f);

            Assert.IsTrue(timeline.UpdateLength(2f));
            Assert.AreEqual(2f, timeline.Length, 1e-5f);

            timeline.Restart();
            LogAssert.Expect(LogType.Warning, new Regex("can't update length"));
            Assert.IsFalse(timeline.UpdateLength(3f), "运行中修改长度应被拒绝");
            Assert.AreEqual(2f, timeline.Length, 1e-5f);
        }

        [Test]
        public void ZeroLengthTimeline_CompletesOnFirstUpdate()
        {
            var timeline = new Timeline(0f);
            var ends = 0;
            timeline.EndAction += _ => ends++;

            timeline.Restart();
            timeline.OnUpdate(0.1f);

            Assert.AreEqual(1, ends);
            Assert.IsFalse(timeline.IsRunning);
            Assert.AreEqual(1f, timeline.NormalizedTime, 1e-5f);
        }

        // ---------- 修复回归 ----------

        [Test]
        public void UpdateLength_Negative_IsRejected_AndLengthUnchanged()
        {
            var timeline = new Timeline(1f);

            LogAssert.Expect(LogType.Warning, new Regex("can't be negative"));
            Assert.IsFalse(timeline.UpdateLength(-1f));
            Assert.AreEqual(1f, timeline.Length, 1e-5f);
        }

        [Test]
        public void UpdateLength_Shrink_ClampsTime()
        {
            var timeline = new Timeline(2f);
            timeline.SetTime(1.5f);

            Assert.IsTrue(timeline.UpdateLength(1f));
            Assert.AreEqual(1f, timeline.Time, 1e-5f);
        }

        [Test]
        public void ZeroLengthTimeline_UpdateAction_ReceivesOne()
        {
            var timeline = new Timeline(0f);
            var normalized = float.NaN;
            timeline.UpdateAction = value => normalized = value;

            timeline.Restart();
            timeline.OnUpdate(0.1f);

            Assert.AreEqual(1f, normalized, 1e-5f, "零长度时不应得到 NaN");
        }

        [Test]
        public void End_IsIdempotent_UntilRestart()
        {
            var timeline = new Timeline(1f);
            var ends = 0;
            timeline.EndAction += _ => ends++;

            timeline.End();
            timeline.End();
            Assert.AreEqual(1, ends, "重复 End 只应触发一次");

            timeline.Restart();
            timeline.End();
            Assert.AreEqual(2, ends, "Restart 后可再次结束");
        }

        [Test]
        public void EndEarly_WhenPaused_StillFiresEnd()
        {
            var timeline = new Timeline(1f);
            var ends = 0;
            timeline.EndAction += _ => ends++;

            Assert.IsFalse(timeline.IsRunning);
            timeline.EndEarly();

            Assert.AreEqual(1, ends);
            Assert.IsFalse(timeline.IsRunning);
            Assert.AreEqual(1f, timeline.Time, 1e-5f);
        }

        [Test]
        public void Start_AfterEnd_ResetsAndCanEndAgain()
        {
            var timeline = new Timeline(1f);
            var ends = 0;
            timeline.EndAction += _ => ends++;

            timeline.Restart();
            timeline.OnUpdate(0.5f);
            timeline.End();
            Assert.AreEqual(1, ends);

            timeline.Start();
            Assert.AreEqual(0f, timeline.Time, 1e-5f, "End 后 Start 应开始新一轮");

            timeline.OnUpdate(0.5f);
            timeline.OnUpdate(0.5f);
            timeline.OnUpdate(0f);

            Assert.AreEqual(2, ends, "新一轮应能再次触发 End");
        }

        [Test]
        public void EndEarly_AfterEnd_DoesNotReplayCallbacks()
        {
            var timeline = new Timeline(1f);
            var ends = 0;
            var updates = 0;
            timeline.UpdateAction += _ => updates++;
            timeline.EndAction += _ => ends++;

            timeline.Restart();
            timeline.OnUpdate(1.2f);
            timeline.OnUpdate(0f);
            Assert.AreEqual(2, updates);
            Assert.AreEqual(1, ends);

            timeline.EndEarly();
            Assert.AreEqual(2, updates, "结束后 EndEarly 不应重复 UpdateAction");
            Assert.AreEqual(1, ends, "结束后 EndEarly 不应重复 EndAction");
        }

        [Test]
        public void NormalizedTime_IsClamped_WhenDeltaOvershoots()
        {
            var timeline = new Timeline(1f);
            float endNormalized = -1f;
            timeline.EndAction += ctx => endNormalized = ctx.NormalizedTime;

            timeline.Restart();
            timeline.OnUpdate(1.5f); // 越过终点
            timeline.OnUpdate(0f);   // 下一拍结束，Context 归一化应被钳制

            Assert.AreEqual(1f, endNormalized, 1e-5f);
            Assert.LessOrEqual(timeline.NormalizedTime, 1f);
        }

        [Test]
        public void RangeEvent_StartingAtZero_FiresOnFirstUpdate()
        {
            var timeline = new Timeline(1f);
            var fired = 0;
            timeline.AddRangeEvent(0f, 1f, _ => fired++);

            timeline.Restart();
            timeline.OnUpdate(0.1f);

            Assert.AreEqual(1, fired, "闭区间起点应包含归一化 0");
        }

        [Test]
        public void RangeEvent_SkippedByLargeDelta_FiresOnce()
        {
            var timeline = new Timeline(1f);
            var fired = 0;
            timeline.AddRangeEvent(0.3f, 0.2f, _ => fired++);

            timeline.Restart();
            timeline.OnUpdate(0.25f); // 0 → 0.25
            timeline.OnUpdate(0.4f);  // 0.25 → 0.65，整段跨过 [0.3, 0.5]
            timeline.OnUpdate(0f);    // 在 0.65 处补一次

            Assert.AreEqual(1, fired, "被整帧跨过的区间应至少触发一次");
            timeline.OnUpdate(0f);
            Assert.AreEqual(1, fired, "补触发后不应重复");
        }

        [Test]
        public void PointEvent_InsertedWhileRunning_DoesNotReplayExecutedEvents()
        {
            var timeline = new Timeline(1f);
            var replay = 0;
            var inserted = 0;
            var future = 0;
            timeline.AddPointEvent(0.2f, _ => replay++);
            timeline.AddPointEvent(0.8f, _ => future++);

            timeline.Restart();
            timeline.OnUpdate(0.5f); // ctx 0
            timeline.OnUpdate(0.2f); // ctx 0.5 → 触发 0.2
            Assert.AreEqual(1, replay);

            timeline.AddPointEvent(0.1f, _ => inserted++);
            for (var i = 0; i < 10; i++)
                timeline.OnUpdate(0.1f);

            Assert.AreEqual(1, replay, "已执行的点事件不应重放");
            Assert.AreEqual(0, inserted, "已越过时刻的新增事件不应补触发");
            Assert.AreEqual(1, future);
        }

        [Test]
        public void PointExecutor_AddEvents_MergesWithExisting()
        {
            var executor = new PointEventsExecutor();
            var first = new TestPointEvent(0.2f);
            var second = new TestPointEvent(0.8f);

            executor.AddEvents(new ITimelineEvent[] { first });
            executor.AddEvents(new ITimelineEvent[] { second });

            executor.Execute(Ctx(0.5f));
            Assert.AreEqual(1, first.Executed);
            Assert.AreEqual(0, second.Executed);

            executor.Execute(Ctx(0.9f));
            Assert.AreEqual(1, second.Executed);
        }

        [Test]
        public void RangeExecutor_AddEvents_MergesWithExisting()
        {
            var executor = new RangeEventsExecutor();
            var first = new TestRangeEvent(0.2f, 0.1f);
            var second = new TestRangeEvent(0.8f, 0.1f);

            executor.AddEvents(new ITimelineEvent[] { first });
            executor.AddEvents(new ITimelineEvent[] { second });

            executor.Execute(Ctx(0.3f));
            Assert.AreEqual(1, first.Executed, "第一次 AddEvents 的事件不应被覆盖");
            Assert.AreEqual(0, second.Executed);

            executor.Execute(Ctx(0.9f));
            Assert.AreEqual(1, second.Executed);
        }

        [Test]
        public void RangeExecutor_RemoveEvent_OnEmpty_DoesNotThrow_AndRemoveEventsRemoves()
        {
            var executor = new RangeEventsExecutor();
            var evt = new TestRangeEvent(0f, 1f);
            Assert.IsFalse(executor.RemoveEvent(evt), "空执行器删除不应抛异常");

            var other = new TestRangeEvent(0f, 1f);
            executor.AddEvents(new ITimelineEvent[] { evt, other });
            Assert.AreEqual(1, (int)executor.RemoveEvents(new ITimelineEvent[] { evt }));

            executor.Execute(Ctx(0.5f));
            Assert.AreEqual(0, evt.Executed);
            Assert.AreEqual(1, other.Executed);
        }

        static TimelineContext Ctx(float normalized) => new TimelineContext
        {
            Time = normalized,
            Duration = 1f,
            NormalizedTime = normalized,
            DeltaTime = 0.1f,
        };

        class TestPointEvent : PointEvent
        {
            public int Executed;
            public TestPointEvent(float proportion) : base(proportion) { }
            public override void Execute(TimelineContext context) => Executed++;
        }

        class TestRangeEvent : RangeEvent
        {
            public int Executed;
            public TestRangeEvent(float triggerProportion, float durationProportion)
                : base(durationProportion, triggerProportion) { }
            public override void Execute(TimelineContext context) => Executed++;
        }
    }
}
