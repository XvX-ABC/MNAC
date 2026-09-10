using MNAC.TPhysics.Environment;
using NUnit.Framework;
using UnityEngine;
using LContext = MNAC.TPhysics.Locomotion.Context;

namespace MNAC.Tests.TPhysics
{
    /// 世界坐标纯逻辑测试。
    public class World_Test
    {
        [Test]
        public void Default_HasIdentityAxes()
        {
            var w = MNAC.TPhysics.World.Default;
            Assert.AreEqual(Vector3.up, w.Up);
            Assert.AreEqual(Vector3.right, w.Right);
            Assert.AreEqual(Vector3.forward, w.Forward);
            Assert.AreEqual(Quaternion.identity, w.Rotation);
        }

        [Test]
        public void TransformVector_RoundTrips_WithInverse()
        {
            var w = new MNAC.TPhysics.World(new Vector3(1, 2, 3).normalized, Vector3.down * 9.81f);
            var v = new Vector3(1.5f, -2f, 3.5f);
            var round = w.TransformVector3(w.InverseTransformVector(v));
            Assert.That((round - v).magnitude, Is.LessThan(1e-4f));
        }

        [Test]
        public void CustomUp_WorldPlaneIsPerpendicular_ToUp()
        {
            var up = new Vector3(0.2f, 1f, 0.1f).normalized;
            var w = new MNAC.TPhysics.World(up, Vector3.down * 9.81f);
            Assert.That(Vector3.Dot(w.Right, w.Up), Is.LessThan(1e-4f));
            Assert.That(Vector3.Dot(w.Forward, w.Up), Is.LessThan(1e-4f));
        }
    }

    /// 物理快照上下文（读侧）测试。
    public class TPhysicsContext_Test
    {
        GameObject _go;
        Rigidbody _rb;

        [SetUp]
        public void SetUp()
        {
            _go = new GameObject();
            _rb = _go.AddComponent<Rigidbody>();
        }

        [TearDown]
        public void TearDown() => Object.DestroyImmediate(_go);

        [Test]
        public void VelocitySet_IncrementsUpdatedCount()
        {
            var ctx = new MNAC.TPhysics.Context(_rb);
            ctx.ResetUpdatedCount();
            Assert.AreEqual(0, ctx.UpdatedCount);

            ctx.CurrentVelocity = new Vector3(1, 2, 3);
            Assert.Greater(ctx.UpdatedCount, 0);
            Assert.AreEqual(new Vector3(1, 2, 3), ctx.CurrentVelocity);
            Assert.AreEqual(ctx.CurrentVelocity.magnitude, ctx.CurrentSpeed, 1e-4f);
        }

        [Test]
        public void SynchronizeFromRigidbody_ReadsCurrentState()
        {
            _rb.velocity = new Vector3(3, 0, 4);
            _rb.position = new Vector3(10, 20, 30);
            var ctx = new MNAC.TPhysics.Context(_rb);

            Assert.AreEqual(new Vector3(3, 0, 4), ctx.CurrentVelocity);
            Assert.AreEqual(new Vector3(10, 20, 30), ctx.CurrentPosition);
            // 注：SynchronizeFromRigidbody 只同步 v/p/r，不刷新 speed（speed 在 CurrentVelocity setter 里算）。
        }

        [Test]
        public void OriginalInfo_ReflectsRigidbody()
        {
            var ctx = new MNAC.TPhysics.Context(_rb);
            Assert.AreEqual(_rb.rotation, ctx.Original.Rotation);
            Assert.AreEqual(_rb.position, ctx.Original.Position);
        }
    }

    /// 环境辅助纯逻辑测试。
    public class EnvironmentHelpers_Test
    {
        [Test]
        public void Ground_MaxSlopeCos_45Degrees()
        {
            Assert.That(Ground.MaxSlopeCos(45f), Is.EqualTo(Mathf.Cos(45f * Mathf.Deg2Rad)).Within(1e-5f));
        }

        [Test]
        public void GroundDetector_MaxSlope_ClampedTo_0_90()
        {
            var d = new GroundDetector(45f, LayerMask.GetMask("Default"));
            d.MaxSlope = 120f;
            Assert.AreEqual(90f, d.MaxSlope);
            d.MaxSlope = -10f;
            Assert.AreEqual(0f, d.MaxSlope);
        }

        [Test]
        public void EnvironmentSettings_Defaults()
        {
            var s = new EnvironmentSettings();
            Assert.AreEqual(Vector3.up, s.WorldUp);
            Assert.AreEqual(45f, s.MaxSlope);
            Assert.AreEqual(1f, s.MaxGroundDistance);
        }
    }

    /// 常开观察模块：竖直速度均值 → 姿态分类。
    public class VerticalPostureEvaluator_Test
    {
        GameObject _go;
        Rigidbody _rb;
        GroundDetector _gd;
        LContext _lctx;

        [SetUp]
        public void SetUp()
        {
            _go = new GameObject();
            _rb = _go.AddComponent<Rigidbody>();
            _gd = new GroundDetector(45f, LayerMask.GetMask("Default"));
            _lctx = new LContext(new MNAC.TPhysics.Context(_rb), _gd);
        }

        [TearDown]
        public void TearDown() => Object.DestroyImmediate(_go);

        LContext Feed(Vector3 worldVelocity, int samples)
        {
            var evaluator = new MNAC.TPhysics.Locomotion.VerticalPostureEvaluator(samples);
            _lctx.CurrentVelocity = worldVelocity;
            return evaluator.OnUpdate(_lctx);
        }

        [Test]
        public void RisingVelocity_YieldsAscending()
        {
            var after = Feed(new Vector3(0, 4f, 0), 3);
            Assert.AreEqual(MNAC.TPhysics.Locomotion.VerticalPosture.Ascending, after.VerticalPosture);
        }

        [Test]
        public void FallingVelocity_YieldsDescending()
        {
            var after = Feed(new Vector3(0, -4f, 0), 3);
            Assert.AreEqual(MNAC.TPhysics.Locomotion.VerticalPosture.Descending, after.VerticalPosture);
        }

        [Test]
        public void LevelVelocity_YieldsHolding()
        {
            var after = Feed(new Vector3(2f, 0f, 0f), 3);
            Assert.AreEqual(MNAC.TPhysics.Locomotion.VerticalPosture.Holding, after.VerticalPosture);
        }
    }

    /// 水平加速模块中不依赖 deltaTime 的确定性分支。
    public class HorizontalLocomotion_Test
    {
        GameObject _go;
        Rigidbody _rb;
        GroundDetector _gd;
        LContext _lctx;

        [SetUp]
        public void SetUp()
        {
            _go = new GameObject();
            _rb = _go.AddComponent<Rigidbody>();
            _gd = new GroundDetector(45f, LayerMask.GetMask("Default"));
            _lctx = new LContext(new MNAC.TPhysics.Context(_rb), _gd);
        }

        [TearDown]
        public void TearDown() => Object.DestroyImmediate(_go);

        [Test]
        public void ZeroAcceleration_SnapsToMaxSpeed_AlongDirection()
        {
            var module = new MNAC.TPhysics.Locomotion.HorizontalLocomotion(0f, 0f, Vector3.zero)
            {
                MaxSpeed = 5f,
                AcceleratedSpeed = 0f,
                DirectionVector = Vector3.right,
            };
            _lctx.CurrentVelocity = Vector3.zero;
            var after = module.OnUpdate(_lctx);
            var v = after.CurrentVelocity;
            Assert.AreEqual(5f, v.magnitude, 1e-3f);
            Assert.Greater(Vector3.Dot(v.normalized, Vector3.right), 0.99f);
        }
    }
}
