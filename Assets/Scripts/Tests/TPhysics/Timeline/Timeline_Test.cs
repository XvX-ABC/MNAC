using System.Text.RegularExpressions;
using MNAC.Utilities.Timeline;
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

            // 归一化时刻 0,.1,...,.9,1.0；区间 (0.3, 0.5] 内为 .4、.5
            Assert.AreEqual(2, fired);
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
    }
}
