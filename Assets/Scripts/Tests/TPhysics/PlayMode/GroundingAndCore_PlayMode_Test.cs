using System.Collections;
using MNAC.TPhysics.Environment;
using MNAC.TPhysics.Locomotion;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace MNAC.Tests.TPhysics.PlayMode
{
    /// 把刚体的碰撞回调转发给纯逻辑 GroundDetector（模拟原 Mono 壳的接线）。
    class CollisionForwarder : MonoBehaviour
    {
        public GroundDetector Detector;
        void OnCollisionEnter(Collision c) => Detector?.OnCollisionEnter(c);
        void OnCollisionStay(Collision c) => Detector?.OnCollisionStay(c);
        void OnCollisionExit(Collision c) => Detector?.OnCollisionExit(c);
    }

    /// 记录生命周期调用序的间谍模块。
    class SpyModule : LocomotionModuleBase
    {
        public int StartCount;
        public int UpdateCount;
        public int EndCount;
        public Vector3 StartVelocity;
        public override MNAC.TPhysics.Locomotion.Context OnStart(MNAC.TPhysics.Locomotion.Context c)
        {
            StartCount++;
            c.CurrentVelocity = StartVelocity;
            return c;
        }
        public override MNAC.TPhysics.Locomotion.Context OnUpdate(MNAC.TPhysics.Locomotion.Context c)
        {
            UpdateCount++;
            return c;
        }
        public override MNAC.TPhysics.Locomotion.Context OnEnd(MNAC.TPhysics.Locomotion.Context c)
        {
            EndCount++;
            return c;
        }
    }

    public class GroundingAndCore_PlayMode_Test
    {
        GameObject _floor;
        GameObject _body;
        GroundDetector _detector;

        [TearDown]
        public void Cleanup()
        {
            if (_body != null) Object.Destroy(_body);
            if (_floor != null) Object.Destroy(_floor);
        }

        [UnityTest]
        public IEnumerator FallingBody_OnFloor_BecomesGrounded_WithUpNormal()
        {
            // 静态地面 + 动态方块
            _floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _floor.name = "Floor";
            _floor.transform.position = new Vector3(0f, -1f, 0f);
            _floor.transform.localScale = new Vector3(20f, 1f, 20f);

            _body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _body.name = "Body";
            _body.transform.localScale = Vector3.one * 0.5f;
            _body.transform.position = new Vector3(0f, 3f, 0f);

            var rb = _body.AddComponent<Rigidbody>();
            rb.useGravity = true;

            _detector = new GroundDetector(45f, LayerMask.GetMask("Default"));
            _detector.Enabled = true;
            _body.AddComponent<CollisionForwarder>().Detector = _detector;

            // 等待落地
            var grounded = false;
            var elapsed = 0f;
            while (elapsed < 4f && !grounded)
            {
                _detector.Position = rb.position;
                _detector.OnFixedUpdate();
                yield return new WaitForFixedUpdate();
                elapsed += Time.fixedDeltaTime;
                grounded = _detector.IsGrounded;
            }

            Assert.IsTrue(grounded, "方块落到地面后应被判定为接地");
            Assert.Less(rb.position.y, 1.5f, "方块应已落到地面附近");
            Assert.AreEqual(0f, _detector.GroundDistance, 1e-3f);
            Assert.Greater(Vector3.Dot(_detector.GroundsNormal, Vector3.up), 0.99f, "平地的支撑法线应朝上");
        }

        [UnityTest]
        public IEnumerator CoreUpdate_AppliesModuleVelocity_AndLifecycleRuns()
        {
            var go = new GameObject("Player");
            var rb = go.AddComponent<Rigidbody>();
            rb.useGravity = false;

            var detector = new GroundDetector(45f, LayerMask.GetMask("Default"));
            var core = new LocomotionCore(new MNAC.TPhysics.World(), rb, detector);

            var spy = new SpyModule { Enabled = true, StartVelocity = new Vector3(0f, 6f, 0f) };
            core.SetModules(new ILocomotionModule[] { spy });

            // 第 1 拍：状态机 step0→Start（Start 后同帧不再 Update），写回刚体
            core.Update();
            Assert.AreEqual(1, spy.StartCount, "启用模块首次 Update 应调用 Start");
            Assert.AreEqual(0, spy.UpdateCount, "Start 帧不执行 Update");
            Assert.AreEqual(0, spy.EndCount);
            Assert.Greater(rb.velocity.y, 0f, "模块设置的竖直速度应被 Apply 写回刚体");

            // 第 2 拍：step1→Update
            core.Update();
            Assert.AreEqual(1, spy.UpdateCount, "第二次 Update 才执行模块 Update");
            Assert.AreEqual(0, spy.EndCount);

            // 禁用后第 3 拍：触发 End，不再 Update
            core.DisableModule(spy);
            core.Update();
            Assert.AreEqual(1, spy.EndCount, "禁用中的模块应触发一次 End");
            Assert.AreEqual(1, spy.UpdateCount, "End 帧不应再 Update");

            // 再次启用：第 4 拍重新 Start
            core.EnableModule(spy);
            core.Update();
            Assert.AreEqual(2, spy.StartCount);

            yield return null;
            Object.Destroy(go);
        }
    }
}
